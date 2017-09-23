using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common.Extensions;
using Papyrus.Language.Parsing;
using Papyrus.Language.ScriptMembers;
using Papyrus.Language.Tokens.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Media;

namespace Papyrus.Language.Tokens {
    public sealed class PapyrusScriptObject : IPapyrusToken, ISyntaxColorableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.ScriptObject; } }
        public PapyrusScript Script { get; private set; }
        public int TokenSize { get { return Script.ScriptName.Length; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsLineExtension { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }

        private PapyrusScriptObject(PapyrusScript script) {
            this.Script = script;
        }
        public static PapyrusScriptObject Get(string scriptName) {
            PapyrusScript script = PapyrusScriptManager.Instance.GetScript(scriptName);
            if (script != null) {
                return new PapyrusScriptObject(script);
            }
            return null;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusScriptObjectColorFormat.Name);
        }
    }

    internal sealed class PapyrusScriptObjectParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                int nextDelim = PapyrusDelimiter.FindNext(context.Source, 0);
                PapyrusScriptObject scriptObject = PapyrusScriptObject.Get(context.Source.TryRemove(nextDelim));
                if (scriptObject != null) {
                    token = scriptObject;
                    return true;
                }
            }

            token = null;
            return false;
        }
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PapyrusScriptObjectColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusScriptObject";

        internal PapyrusScriptObjectColorFormat() {
            DisplayName = "Papyrus Script Object";
            ForegroundColor = Color.FromRgb(78, 201, 176);
            IsBold = true;
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusScriptObjectColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusScriptObjectColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
