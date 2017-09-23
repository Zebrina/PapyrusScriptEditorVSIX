using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Papyrus.Language_NEW.Data;
using Papyrus.Language_NEW.Tokens;
using Papyrus.Language_NEW.Tokens.Interfaces;
using Papyrus.Language_NEW.Parsing;
using Papyrus.Language_NEW.Parsing.Interfaces;

// READY
namespace Papyrus.Language_NEW.Tokens {
    public class PapyrusStringLiteral : IPapyrusToken, ISyntaxColorableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.String; } }
        public string Value { get; private set; }
        public int TokenSize { get { return Value.Length + 2; } }
        public bool IsCompileTimeConstant { get { return true; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusStringLiteral(string value) {
            this.Value = value != null ? value : "";
        }

        public bool IsEqualToToken(IPapyrusToken other) {
            return other is PapyrusStringLiteral && Value.Equals(((PapyrusStringLiteral)other).Value, StringComparison.OrdinalIgnoreCase);
        }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo textFormatInfo) {
            stringBuilder.Append('"');
            stringBuilder.Append(Value);
            stringBuilder.Append('"');
            return true;
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
        public bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, out IPapyrusToken token) {
            if (scanner.CurrentState == TokenScannerState.Default) {
                if (sourceTextSpan.FirstOrDefault() == '"') {
                    int length = PapyrusDelimiter.FindNext(sourceTextSpan, 1, '"');
                    token = new PapyrusStringLiteral(sourceTextSpan.Substring(1, Math.Min(length, sourceTextSpan.Length) - 1));
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
