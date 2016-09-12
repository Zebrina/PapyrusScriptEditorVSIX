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
    internal sealed class CommentColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusComment";

        internal CommentColorFormat() {
            DisplayName = "Papyrus Comment";
            ForegroundColor = Color.FromRgb(87, 166, 74);
        }
    }

    internal static class CommentColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(CommentColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public class Comment : TokenType, ISyntaxColorable {
        public const string BlockBegin = ";/";
        public const string BlockEnd = "/;";

        private string text;
        private bool isBlock;

        public Comment(string text, bool isBlock) {
            this.text = text;
            this.isBlock = isBlock;
        }

        public override string Text {
            get { return text; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Comment; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Comment; }
        }

        public override bool IgnoredInLine {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(CommentColorFormat.Name);
        }
    }
}
