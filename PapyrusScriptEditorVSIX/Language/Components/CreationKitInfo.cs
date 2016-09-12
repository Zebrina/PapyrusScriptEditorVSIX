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
    internal sealed class CreationKitDocumentationColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusCreationKitDocumentation";

        internal CreationKitDocumentationColorFormat() {
            DisplayName = "Papyrus Creation Kit Documentation";
            ForegroundColor = Color.FromRgb(96, 139, 78);
        }
    }

    internal static class CreationKitDocumentationColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(CreationKitDocumentationColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public class CreationKitDocumentation : TokenType, ISyntaxColorable {
        private string documentationText;

        public CreationKitDocumentation(string documentationText) {
            Debug.Assert(documentationText != null);

            this.documentationText = documentationText;
        }

        public override string Text {
            get { return documentationText; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.CreationKitInfo; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.CreationKitInfo; }
        }

        public override bool IgnoredInLine {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(CreationKitDocumentationColorFormat.Name);
        }
    }
}
