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
    internal static class StringLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(StringLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class StringLiteralColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusStringLiteral";

        internal StringLiteralColorFormat() {
            DisplayName = "Papyrus String";
            ForegroundColor = Color.FromRgb(214, 157, 133);
        }
    }

    //[DebuggerStepThrough]
    public sealed class StringLiteral : Token, ISyntaxColorable {
        private string value;

        public StringLiteral(string value) {
            this.value = value;
        }

        public override string Text {
            get { return String.Format("\"{0}\"", value); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.String; }
        }

        public override bool IsCompileTimeConstant {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(StringLiteralColorFormat.Name);
        }

        public override int GetHashCode() {
            return Text.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is StringLiteral && String.Equals(this.value, ((StringLiteral)obj).value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(StringLiteral x, StringLiteral y) {
            return Equals(x, y);
        }
        public static bool operator !=(StringLiteral x, StringLiteral y) {
            return !Equals(x, y);
        }
        public static bool operator ==(StringLiteral x, Token y) {
            return x == y as StringLiteral;
        }
        public static bool operator !=(StringLiteral x, Token y) {
            return x != y as StringLiteral;
        }
        public static bool operator ==(Token x, StringLiteral y) {
            return x as StringLiteral == y;
        }
        public static bool operator !=(Token x, StringLiteral y) {
            return x as StringLiteral != y;
        }
    }

    internal sealed class StringLiteralParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token) {
            if (state == TokenScannerState.Text) {
                if (sourceTextSpan.FirstOrDefault() == Characters.QuotationMark) {
                    int length = Delimiter.FindNext(sourceTextSpan, 1, Characters.QuotationMark);
                    token = new StringLiteral(sourceTextSpan.Substring(1, Math.Min(length, sourceTextSpan.Length) - 1));
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
