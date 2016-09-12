#if false
using Papyrus.Common;
using Papyrus.Language.Components;
using Papyrus.Language.Dissecting;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class ScriptParser {
        private const string ReturnTypeKey = "ReturnType";
        private const string PropertyTypeKey = "PropertyType";
        private const string ParametersKey = "Parameters";
        private const string AttributesKey = "Attributes";

        private readonly IEnumerable<string> sourceFolders;

        [DebuggerStepThrough]
        public ScriptParser(params string[] sourceFolders) {
            this.sourceFolders = sourceFolders;
        }
        [DebuggerStepThrough]
        public ScriptParser(IEnumerable<string> sourceFolders) {
            this.sourceFolders = new List<string>(sourceFolders);
        }

        public ScriptObject ParseScript(string scriptName, bool reParse = false) {
            string fileName = FindScriptFile(scriptName);
            if (fileName != null) {
                return ParseScriptFile(null, fileName, false);
            }
            return null;
        }
        public void ParseAllScripts() {
            foreach (string folder in sourceFolders) {
                foreach (string file in Directory.GetFiles(folder, "*.psc", SearchOption.TopDirectoryOnly)) {
                    ParseScriptFile(null, file, false);
                }
            }
        }

        private string FindScriptFile(string scriptName) {
            // Append file extensions if not present.
            if (!scriptName.EndsWith(PapyrusContentDefinition.FileExtension, StringComparison.OrdinalIgnoreCase)) {
                scriptName += PapyrusContentDefinition.FileExtension;
            }

            foreach (string folder in sourceFolders) {
                string fullFileName = String.Format("{0}\\{1}", folder, scriptName);
                if (File.Exists(fullFileName)) {
                    return fullFileName;
                }
            }

            return null;
        }

        private enum ParserState {
            ParseInfo,
            ParseMembers,
            InsideProperty,
            InsideFunction,
            InsideEvent,
            // FO4
            InsideStruct,
        }
        private object parseLock = new object();
        private ScriptObject ParseScriptFile(IDictionary<string, ScriptObject> collection, string scriptFile, bool reParse) {
            ScriptObject scriptObject = null;

            string scriptName = Path.GetFileNameWithoutExtension(scriptFile);

            scriptObject = ScriptObjectManager.Instance.Get(scriptName);
            if (scriptObject != null) {
                if (reParse) {
                    scriptObject.Clear();
                }
                else {
                    return scriptObject;
                }
            }

            ParserState state = ParserState.ParseInfo;
            TokenParser tokenParser = new TokenParser();
            ParsedLine parsedLine = new ParsedLine();

            using (StreamReader stream = new StreamReader(scriptFile)) {
                TokenParserResult result;
                while ((result = tokenParser.ParseLine(stream, parsedLine)) != TokenParserResult.EndSource) {
                    if (result != TokenParserResult.ExtendedLine && parsedLine.Count > 0) {
                        switch (state) {
                            case ParserState.ParseInfo:
                                ScriptInfo info = new ScriptInfo();
                                if (!ParseScriptInfo(parsedLine, ref info)) {
                                    return null;
                                }

                                scriptObject = ScriptObjectManager.Instance.Get(scriptName);
                                if (scriptObject == null) {
                                    scriptObject = new ScriptObject(info);
                                    ScriptObjectManager.Instance.Add(scriptObject);
                                }
                                else {
                                    scriptObject.Info = info;
                                }

                                state = ParserState.ParseMembers;

                                break;

                            case ParserState.InsideProperty:
                                if (parsedLine.Count == 1 && TokenType.Equals(parsedLine.First(), Keyword.EndProperty)) {
                                    state = ParserState.ParseMembers;
                                }

                                break;
                            case ParserState.InsideFunction:
                                if (parsedLine.Count == 1 && TokenType.Equals(parsedLine.First(), Keyword.EndFunction)) {
                                    state = ParserState.ParseMembers;
                                }

                                break;
                            case ParserState.InsideEvent:
                                if (parsedLine.Count == 1 && TokenType.Equals(parsedLine.First(), Keyword.EndEvent)) {
                                    state = ParserState.ParseMembers;
                                }

                                break;

                            case ParserState.ParseMembers:
                                if (ParseProperty(parsedLine, ref scriptObject)) {
                                    if (Keyword.Property.IsOutlineableStart(parsedLine)) {
                                        state = ParserState.InsideProperty;
                                    }
                                }
                                else if (ParseFunction(parsedLine, ref scriptObject)) {
                                    if (Keyword.Function.IsOutlineableStart(parsedLine)) {
                                        state = ParserState.InsideFunction;
                                    }
                                }
                                else if (ParseEvent(parsedLine, ref scriptObject)) {
                                    if (Keyword.Event.IsOutlineableStart(parsedLine)) {
                                        state = ParserState.InsideEvent;
                                    }
                                }

                                break;
                        }

                        parsedLine.Clear();
                    }
                }
            }

            return scriptObject;
        }

        private bool ParseScriptInfo(IReadOnlyParsedLine parsedLine, ref ScriptInfo info) {
            DissectedLine dissectedLine = new DissectedLine();
            ILineDissector dissector = new ScriptInfoDissector(AttributesKey);

            if (dissector.DissectLine(this, parsedLine, 0, dissectedLine) == -1) {
                return false;
            }

            info.Name = dissectedLine.GetEntry(Keyword.Scriptname).Text;
            info.Parent = (ScriptObject)dissectedLine.GetEntry(Keyword.Extends);
            info.Conditional = dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Conditional);
            info.Hidden = dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Hidden);

            return true;
        }

        private bool ParseProperty(IReadOnlyParsedLine parsedLine, ref ScriptObject scriptObject) {
            DissectedLine dissectedLine = new DissectedLine();
            ILineDissector dissector = new PropertyDissector(ReturnTypeKey, PropertyTypeKey, AttributesKey);

            if (dissector.DissectLine(this, parsedLine, 0, dissectedLine) == -1) {
                return false;
            }

            scriptObject.Add(new AutoPropertyMember(dissectedLine.GetEntry(ReturnTypeKey),
                dissectedLine.GetEntry(Keyword.Property) as Identifier,
                dissectedLine.GetEntry(Operator.BasicAssignment),
                dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.AutoReadOnly),
                dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Conditional),
                dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Hidden)));

            return true;
        }

        private bool ParseFunction(IReadOnlyParsedLine parsedLine, ref ScriptObject scriptObject) {
            DissectedLine dissectedLine = new DissectedLine();
            ILineDissector dissector = new FunctionDissector(ReturnTypeKey, ParametersKey, AttributesKey);

            if (dissector.DissectLine(this, parsedLine, 0, dissectedLine) == -1) {
                return false;
            }

            scriptObject.Add(new FunctionMember(dissectedLine.GetEntry(ReturnTypeKey),
                dissectedLine.GetEntry(Keyword.Function).Text,
                dissectedLine.GetArrayEntries(ParametersKey).Select(t => t as Parameter),
                dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Global),
                dissectedLine.ContainsArrayEntry(AttributesKey, Keyword.Native)));

            return true;
        }

        private bool ParseEvent(IReadOnlyParsedLine parsedLine, ref ScriptObject scriptObject) {
            DissectedLine dissectedLine = new DissectedLine();
            ILineDissector dissector = new EventDissector(ParametersKey);

            if (dissector.DissectLine(this, parsedLine, 0, dissectedLine) == -1) {
                return false;
            }

            scriptObject.Add(new EventMember(dissectedLine.GetEntry(Keyword.Event).Text,
                dissectedLine.GetArrayEntries(ParametersKey).Select(t => t as Parameter)));

            return true;
        }

        private bool ParseState(IReadOnlyParsedLine parsedLine, ref ScriptObject scriptObject) {


            return true;
        }

        private bool ParseStruct(IReadOnlyParsedLine parsedLine, ref ScriptObject scriptObject) {


            return true;
        }
    }
}

#endif