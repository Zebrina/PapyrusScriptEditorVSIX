using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using Papyrus.Language.Tokens.Interfaces;
using Papyrus.Language.Parsing;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusStringLiteral : IPapyrusToken, ISyntaxColorableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.String; } }
        public string Value { get; private set; }
        public int TokenSize { get { return Value.Length + 2; } }
        public bool IsCompileTimeConstant { get { return true; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusStringLiteral(string value) {
            this.Value = value != null ? value : "";
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusStringLiteralColorFormat.Name);
        }

        #region Parsing

        public static PapyrusStringLiteral Parse(string source, int position) {
            if (source[position] == '"') {

            }
            return null;
        }

        #endregion
    }

    internal sealed class PapyrusStringLiteralParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                if (context.Source.FirstOrDefault() == '"') {
                    int length = PapyrusDelimiter.FindNext(context.Source, 1, '"');
                    token = new PapyrusStringLiteral(context.Source.Substring(1, Math.Min(length, context.Source.Length) - 1));
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
    internal sealed class PapyrusStringLiteralColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusStringLiteral";

        internal PapyrusStringLiteralColorFormat() {
            DisplayName = "Papyrus String";
            ForegroundColor = Color.FromRgb(214, 157, 133);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusStringLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusStringLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
