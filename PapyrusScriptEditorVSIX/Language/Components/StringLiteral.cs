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
    internal sealed class StringLiteralColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusStringLiteral";

        internal StringLiteralColorFormat() {
            DisplayName = "Papyrus String";
            ForegroundColor = Color.FromRgb(214, 157, 133);
        }
    }

    internal static class StringLiteralColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(StringLiteralColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public sealed class StringLiteral : TokenType, ISyntaxColorable {
        private string value;

        public StringLiteral(string value) {
            this.value = value;
        }

        public override string Text {
            get { return String.Concat('"', value, '"'); ; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.String; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.String; }
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
        public static bool operator ==(StringLiteral x, TokenType y) {
            return x == (StringLiteral)y;
        }
        public static bool operator !=(StringLiteral x, TokenType y) {
            return x != (StringLiteral)y;
        }
    }
}
