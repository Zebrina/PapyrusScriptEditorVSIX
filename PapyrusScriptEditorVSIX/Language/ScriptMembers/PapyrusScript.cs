using Papyrus.Common;
using Papyrus.Common.Extensions;
using Papyrus.Language.Parsing;
using Papyrus.Language.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.ScriptMembers {
    /// <summary>
    /// Top-level 'script member'. Basically the script with all it's properties, functions, events, etc.
    /// All ScriptObject tokens should link to a papyrus script.
    /// </summary>
    public sealed class PapyrusScript : IPapyrusScriptMember {
        private PapyrusScript parentScript;
        private HashSet<string> memberKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<PapyrusScriptMemberType, ICollection<IPapyrusScriptMember>> memberValues;

        public PapyrusScript(string scriptName) {
            this.ScriptName = scriptName;
            this.memberValues = new Dictionary<PapyrusScriptMemberType, ICollection<IPapyrusScriptMember>>();
            foreach (PapyrusScriptMemberType type in Enum.GetValues(typeof(PapyrusScriptMemberType))) {
                memberValues.Add(type, new List<IPapyrusScriptMember>());
            }
        }

        PapyrusScriptMemberType IPapyrusScriptMember.Type {
            get { return PapyrusScriptMemberType.PapyrusScript; }
        }
        public string ScriptName { get; private set; }
        public IEnumerable<IPapyrusScriptMember> GetMembers(string filter) {
            foreach (var memberTypeList in memberValues) {
                foreach (var member in memberTypeList.Value.Where(m => String.IsNullOrWhiteSpace(filter) || m.ScriptName.ContainsIgnoreCase(filter))) {
                    yield return member;
                }
            }
        }
        public bool AddMember(IPapyrusScriptMember member) {
            if (memberKeys.Contains(member.ScriptName)) {
                return false;
            }

            ICollection<IPapyrusScriptMember> members;
            if (!memberValues.TryGetValue(PapyrusScriptMemberType.PapyrusImport, out members)) {
                members = new List<IPapyrusScriptMember>();
                memberValues.Add(PapyrusScriptMemberType.PapyrusImport, members);
            }

            members.Add(member);
            memberKeys.Add(member.ScriptName);

            return true;
        }
        public bool RemoveMember(string memberName) {
            if (memberKeys.Contains(memberName)) {
                IPapyrusScriptMember element = null;
                var memberTypeList = memberValues.First(mList => mList.Value.Any(m => {
                    if (m.ScriptName.Equals(memberName, StringComparison.OrdinalIgnoreCase)) {
                        element = m;
                        return true;
                    }
                    return false;
                }));

                memberTypeList.Value.Remove(element);
                if (memberTypeList.Value.Count == 0) {
                    memberValues.Remove(memberTypeList.Key);
                }

                return true;
            }

            return false;
        }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            return false;
        }
    }

    public sealed class PapyrusScriptManager {
        private static PapyrusScriptManager instance = null;
        private static object syncObj = new object();
        public static PapyrusScriptManager Instance {
            get {
                if (instance == null) {
                    lock (syncObj) {
                        if (instance == null) {
                            instance = new PapyrusScriptManager();
                        }
                    }
                }
                return instance;
            }
        }

        private Dictionary<string, PapyrusScript> loadedScripts = new Dictionary<string, PapyrusScript>(StringComparer.OrdinalIgnoreCase);

        public PapyrusScript GetScript(string scriptName) {
            return GetScript(scriptName, true);
        }
        public PapyrusScript GetScript(string scriptName, bool tryLoad) {
            PapyrusScript script;
            if (loadedScripts.TryGetValue(scriptName, out script)) {
                return script;
            }
            else if (tryLoad) {
                return TryLoadScript(scriptName);
            }
            return null;
        }
        public void AddScript(PapyrusScript papyrusScript) {
            if (!loadedScripts.ContainsKey(papyrusScript.ScriptName)) {
                loadedScripts.Add(papyrusScript.ScriptName, papyrusScript);
            }
        }
        public bool RemoveScript(PapyrusScript papyrusScript) {
            return loadedScripts.Remove(papyrusScript.ScriptName);
        }
        public bool RemoveScript(string scriptName) {
            return loadedScripts.Remove(scriptName);
        }

        public bool LoadScript(string scriptName) {
            return TryLoadScript(scriptName) != null;
        }

        public bool ReloadScript(string scriptName) {
            return false;
        }

        private PapyrusScript TryLoadScript(string scriptName) {
            string fileName = PapyrusEditor.FindSourceFile(scriptName);
            if (!String.IsNullOrEmpty(fileName)) {
                PapyrusScript papyrusScript = new PapyrusScript(scriptName);
                loadedScripts.Add(scriptName, papyrusScript);
                using (StreamReader fileStream = new StreamReader(fileName)) {
                    if (!TryParseScript(fileStream, papyrusScript)) {
                        //loadedScripts.Remove(papyrusScript.ScriptName);
                    }
                    return papyrusScript;
                }
            }
            return null;
        }

        private static bool TryParseScript(StreamReader fileStream, PapyrusScript papyrusScript) {
            TokenScanner scanner = new TokenScanner();
            scanner.AddParser(new PapyrusBlockCommentParser());
            scanner.AddParser(new PapyrusLineCommentParser());
            scanner.AddParser(new PapyrusDocumentationParser());
            scanner.AddParser(new PapyrusStringLiteralParser());
            scanner.AddParser(new PapyrusNumericLiteralParser());
            scanner.AddParser(new PapyrusOperatorParser());
            scanner.AddParser(new PapyrusDelimiterParser());
            scanner.AddParser(new PapyrusKeywordParser());
            scanner.AddParser(new PapyrusScriptObjectParser());
            scanner.AddParser(new PapyrusIdentifierParser());

            var tokenLines = new List<List<IPapyrusToken>>();
            //TokenSnapshot resultTokenSnapshot = new TokenSnapshot(snapshot);
            var parsedLine = new List<IPapyrusToken>();

            string line;
            while (!String.IsNullOrEmpty(line = fileStream.ReadLine())) {
                if (scanner.ScanLine(line, parsedLine) == 0 || parsedLine.Any(t => t.IsLineExtension)) {
                    continue;
                }

                tokenLines.Add(parsedLine);
                parsedLine = new List<IPapyrusToken>();
            }

            // Lots of stuff to do here ...

            return true;
        }
    }
}
