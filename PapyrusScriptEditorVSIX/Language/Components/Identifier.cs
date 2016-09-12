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

    internal static class IdentifierColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(IdentifierColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public sealed class Identifier : TokenType, ISyntaxColorable {
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
        public override TokenColorID Color {
            get { return TokenColorID.Identifier; }
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

        public static implicit operator string(Identifier identifier) {
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
}
