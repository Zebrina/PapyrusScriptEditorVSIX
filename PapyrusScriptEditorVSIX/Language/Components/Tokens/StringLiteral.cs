﻿using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
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

        public override bool CompileTimeConstant {
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
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                if (text.FirstOrDefault() == (char)Delimiter.QuotationMark) {
                    int length = Delimiter.FindNext(text, 1, Delimiter.QuotationMark);
                    token.Type = new StringLiteral(text.Substring(1, length));
                    token.Span = sourceSnapshotSpan.Subspan(0, length + 1);
                    return true;
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                if (sourceTextSpan.FirstOrDefault() == (char)Delimiter.QuotationMark) {
                    int length = Delimiter.FindNext(sourceTextSpan, 1, Delimiter.QuotationMark);
                    token = new StringLiteral(sourceTextSpan.Substring(1, Math.Min(length, sourceTextSpan.Length) - 1));
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
