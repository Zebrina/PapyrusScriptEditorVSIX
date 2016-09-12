using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Papyrus.Language.Components {
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

    internal static class NumericLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(NumericLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    //[DebuggerStepThrough]
    public sealed class NumericLiteral : TokenType, ISyntaxColorable {
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
        public override TokenColorID Color {
            get { return TokenColorID.Literal; }
        }

        public override bool CompileTimeConstant {
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
        public static bool operator ==(NumericLiteral x, TokenType y) {
            return x == (NumericLiteral)y;
        }
        public static bool operator !=(NumericLiteral x, TokenType y) {
            return x != (NumericLiteral)y;
        }

        public static NumericLiteral Parse(string source) {
            if (!String.IsNullOrWhiteSpace(source)) {
                if (TryParseInteger(source) ||
                    TryParseHexNumber(source) ||
                    TryParseFloat(source)) {
                    return new NumericLiteral(source);
                }
            }
            return null;
        }
        
        private const string IntegerDigits = "0123456789";
        private const string HexNumberDigits = IntegerDigits + "aAbBcCdDeEfF";
        private const string HexNumberIdentifier = "xX";

        private enum IntegerParseState {
            Negative,
            FirstDigit,
            AdditionalDigits,
        }
        private static bool TryParseInteger(string source) {
            IntegerParseState state = IntegerParseState.Negative;
            bool result = source.All(c => {
                switch (state) {
                    case IntegerParseState.Negative:
                        if (c == '-') {
                            state = IntegerParseState.FirstDigit;
                            return true;
                        }
                        state = IntegerParseState.AdditionalDigits;
                        return IntegerDigits.Contains(c);

                    case IntegerParseState.FirstDigit:
                        state = IntegerParseState.AdditionalDigits;
                        return IntegerDigits.Contains(c);

                    case IntegerParseState.AdditionalDigits:
                        return HexNumberDigits.Contains(c);

                    default:
                        return false;
                }
            });

            return result && state == IntegerParseState.AdditionalDigits;
        }

        private enum HexNumberParseState {
            Zero,
            HexIdentifier,
            HexDigitAfter,
            AdditionalHexDigits,
        }
        private static bool TryParseHexNumber(string source) {
            HexNumberParseState state = HexNumberParseState.Zero;
            bool result = source.All(c => {
                switch (state) {
                    case HexNumberParseState.Zero:
                        state = HexNumberParseState.HexIdentifier;
                        return c == '0';

                    case HexNumberParseState.HexIdentifier:
                        state = HexNumberParseState.HexDigitAfter;
                        return HexNumberIdentifier.Contains(c);

                    case HexNumberParseState.HexDigitAfter:
                        state = HexNumberParseState.AdditionalHexDigits;
                        return HexNumberDigits.Contains(c);

                    case HexNumberParseState.AdditionalHexDigits:
                        return HexNumberDigits.Contains(c);

                    default:
                        return false;
                }
            });

            return result && state == HexNumberParseState.AdditionalHexDigits;
        }

        private enum FloatParseState {
            FirstCharacter,
            DigitBefore,
            DecimalPoint,
            DigitAfter,
            AdditionalDigits,

        }
        private static bool TryParseFloat(string source) {
            FloatParseState state = FloatParseState.FirstCharacter;
            bool result = source.All(c => {
                switch (state) {
                    case FloatParseState.FirstCharacter:
                        if (c == '-') {
                            state = FloatParseState.DigitBefore;
                            return true;
                        }
                        state = FloatParseState.DecimalPoint;
                        return IntegerDigits.Contains(c);

                    case FloatParseState.DigitBefore:
                        state = FloatParseState.DecimalPoint;
                        return IntegerDigits.Contains(c);

                    case FloatParseState.DecimalPoint:
                        if (c == '.') {
                            state = FloatParseState.DigitAfter;
                            return true;
                        }
                        return IntegerDigits.Contains(c);

                    case FloatParseState.DigitAfter:
                        if (IntegerDigits.Contains(c)) {
                            state = FloatParseState.AdditionalDigits;
                            return true;
                        }
                        return false;

                    case FloatParseState.AdditionalDigits:
                        return IntegerDigits.Contains(c);

                    default:
                        return false;
                }
            });

            return result && state == FloatParseState.AdditionalDigits;
        }
    }
}
