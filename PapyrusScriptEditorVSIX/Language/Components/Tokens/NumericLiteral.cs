using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace Papyrus.Language.Components.Tokens {
    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class NumericLiteralColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusLiteral";

        internal NumericLiteralColorFormat() {
            DisplayName = "Papyrus Literal";
            ForegroundColor = Color.FromRgb(181, 206, 168);
        }
    }

    [DebuggerStepThrough]
    internal static class NumericLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(NumericLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    //[DebuggerStepThrough]
    public sealed class NumericLiteral : Token, ISyntaxColorable {
        private string value;

        public NumericLiteral(string value) {
            this.value = value;
        }

        public override string Text {
            get { return value; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.NumericLiteral; }
        }

        public override bool IsCompileTimeConstant {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(NumericLiteralColorFormat.Name);
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is NumericLiteral && String.Equals(this.value, ((NumericLiteral)obj).value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(NumericLiteral x, NumericLiteral y) {
            return Equals(x, y);
        }
        public static bool operator !=(NumericLiteral x, NumericLiteral y) {
            return !Equals(x, y);
        }
        public static bool operator ==(NumericLiteral x, Token y) {
            return x == y as NumericLiteral;
        }
        public static bool operator !=(NumericLiteral x, Token y) {
            return x != y as NumericLiteral;
        }
        public static bool operator ==(Token x, NumericLiteral y) {
            return x as NumericLiteral == y;
        }
        public static bool operator !=(Token x, NumericLiteral y) {
            return x as NumericLiteral != y;
        }

        public static NumericLiteral Parse(string source) {
            int length;
            if (!String.IsNullOrWhiteSpace(source)) {
                if (TryParseHexNumber(source, out length) ||
                    TryParseFloat(source, out length) ||
                    TryParseInteger(source, out length)) {
                    return new NumericLiteral(length == source.Length ? source : source.Remove(length));
                }
            }
            return null;
        }
        
        private const string IntegerDigits = "0123456789";
        private const string HexNumberDigits = IntegerDigits + "aAbBcCdDeEfF";
        private const string HexNumberIdentifier = "xX";

        private static bool TryParseInteger(string source, out int length) {
            bool negative = source.StartsWith("-");
            for (length = negative ? 1 : 0; length < source.Length; ++length) {
                if (!IntegerDigits.Contains(source[length])) {
                    break;
                }
            }
            return length > (negative ? 1 : 0);
        }

        private static bool TryParseHexNumber(string source, out int length) {
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

        private static bool TryParseFloat(string source, out int length) {
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
    }

    internal sealed class NumericLiteralParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token) {
            if (state == TokenScannerState.Text) {
                NumericLiteral numericLiteral = NumericLiteral.Parse(sourceTextSpan);
                if (numericLiteral != null) {
                    token = numericLiteral;
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
