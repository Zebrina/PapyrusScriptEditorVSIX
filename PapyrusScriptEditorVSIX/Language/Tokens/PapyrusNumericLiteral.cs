using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language.Parsing;
using Papyrus.Language.Tokens;
using Papyrus.Language.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusNumericLiteral : IPapyrusToken, ISyntaxColorableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.NumericLiteral; } }
        public string Value { get; private set; }
        public int TokenSize { get { return Value.Length; } }
        public bool IsCompileTimeConstant { get { return true; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        private PapyrusNumericLiteral(string value) {
            this.Value = value;
        }
        public static PapyrusNumericLiteral Create(string value) {
            int length;
            if (!String.IsNullOrWhiteSpace(value)) {
                if (TryParseHexNumber(value, 0, out length) ||
                    TryParseFloat(value, 0, out length) ||
                    TryParseInteger(value, 0, out length)) {
                    if (length == value.Length) {
                        return new PapyrusNumericLiteral(value.Remove(length));
                    }
                }
            }
            return null;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusNumericLiteralColorFormat.Name);
        }

        #region Parsing

        public static PapyrusNumericLiteral Parse(string source, int position = 0) {
            int length;
            if (!String.IsNullOrWhiteSpace(source)) {
                if (TryParseHexNumber(source, position, out length) ||
                    TryParseFloat(source, position, out length) ||
                    TryParseInteger(source, position, out length)) {
                    return new PapyrusNumericLiteral(source.Substring(position, length));
                }
            }
            return null;
        }

        private const string IntegerDigits = "0123456789";
        private const string HexNumberDigits = IntegerDigits + "aAbBcCdDeEfF";
        private const string HexNumberIdentifier = "xX";

        private static bool TryParseInteger(string source, int position, out int length) {
            if (position > 0) {
                source = source.Substring(position);
            }

            bool negative = source.StartsWith("-");
            for (length = negative ? 1 : 0; length < source.Length; ++length) {
                if (!IntegerDigits.Contains(source[length])) {
                    break;
                }
            }
            return length > (negative ? 1 : 0);
        }

        private static bool TryParseHexNumber(string source, int position, out int length) {
            if (position > 0) {
                source = source.Substring(position);
            }

            if (source.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                for (length = 2; length < source.Length; ++length) {
                    if (!HexNumberDigits.Contains(source[length])) {
                        break;
                    }
                }
                return length > 2;
            }
            length = 0;
            return false;
        }

        private static bool TryParseFloat(string source, int position, out int length) {
            if (position > 0) {
                source = source.Substring(position);
            }

            bool negative = source.StartsWith("-");
            bool decimalPoint = false;
            for (length = negative ? 1 : 0; length < source.Length; ++length) {
                if (!decimalPoint && length > (negative ? 1 : 0) && source[length] == '.') {
                    decimalPoint = true;
                }
                else if (!IntegerDigits.Contains(source[length])) {
                    break;
                }
            }
            return length >= (negative ? 4 : 3);
        }

        #endregion
    }

    internal sealed class PapyrusNumericLiteralParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                PapyrusNumericLiteral numericLiteral = PapyrusNumericLiteral.Parse(context.Source);
                if (numericLiteral != null) {
                    token = numericLiteral;
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
    internal sealed class PapyrusNumericLiteralColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusLiteral";

        internal PapyrusNumericLiteralColorFormat() {
            DisplayName = "Papyrus Literal";
            ForegroundColor = Color.FromRgb(181, 206, 168);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusNumericLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusNumericLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
