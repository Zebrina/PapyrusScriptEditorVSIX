using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
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
    internal sealed class IdentifierColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusIdentifier";

        internal IdentifierColorFormat() {
            DisplayName = "Papyrus Identifier";
            ForegroundColor = Color.FromRgb(220, 220, 220);
        }
    }

    [DebuggerStepThrough]
    internal static class IdentifierColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(IdentifierColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public sealed class Identifier : Token, ISyntaxColorable {
        private string name;

        public Identifier(string name) {
            this.name = name;
        }

        public override string Text {
            get { return name; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Identifier; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(IdentifierColorFormat.Name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Identifier && String.Equals(this.name, ((Identifier)obj).name, StringComparison.OrdinalIgnoreCase);
        }

        public static explicit operator string(Identifier identifier) {
            return identifier.name;
        }
        public static implicit operator Identifier(string value) {
            return new Identifier(value);
        }

        public static bool IsValid(string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                return false;
            }

            bool first = true;
            return value.All(c => {
                if (first) {
                    first = false;
                    return Char.IsLetter(c) || c == '_';
                }
                return Char.IsLetterOrDigit(c) ||c == '_';
            });
        }
    }

    internal sealed class IdentifierParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                int length = Delimiter.FindNext(text, 0);
                if (Identifier.IsValid(text.Substring(0, length))) {
                    token.Type = new Identifier(text.Substring(0, length));
                    token.Span = sourceSnapshotSpan.Subspan(0, length);
                    return true;
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                int length = Delimiter.FindNext(sourceTextSpan, 0);
                if (Identifier.IsValid(sourceTextSpan.Substring(0, length))) {
                    token = new Identifier(sourceTextSpan.Substring(0, length));
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
