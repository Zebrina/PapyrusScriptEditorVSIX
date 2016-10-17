using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
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
    internal sealed class CreationKitDocumentationColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusCreationKitDocumentation";

        internal CreationKitDocumentationColorFormat() {
            DisplayName = "Papyrus Creation Kit Documentation";
            ForegroundColor = Color.FromRgb(96, 139, 78);
        }
    }

    [DebuggerStepThrough]
    internal static class CreationKitDocumentationColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(CreationKitDocumentationColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public sealed class CreationKitDocumentation : Token, ISyntaxColorable {
        private string documentationText;

        public CreationKitDocumentation(string documentationText) {
            this.documentationText = documentationText;
        }

        public override string Text {
            get { return documentationText; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.CreationKitInfo; }
        }

        public override bool IgnoredBySyntax {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(CreationKitDocumentationColorFormat.Name);
        }
    }

    internal sealed class CreationKitDocumentationParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            int endOffset, length;
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                if (text.FirstOrDefault() == (char)Delimiter.LeftCurlyBracket) {
                    endOffset = text.IndexOf((char)Delimiter.RightCurlyBracket, 1);
                    if (endOffset == -1) {
                        state = TokenScannerState.Documentation;
                        token.Type = new CreationKitDocumentation(text);
                        token.Span = sourceSnapshotSpan;
                    }
                    else {
                        length = (endOffset + 1);
                        token.Type = new CreationKitDocumentation(text.Substring(0, length));
                        token.Span = sourceSnapshotSpan.Subspan(0, length);
                    }
                    return true;
                }
            }
            else if (state == TokenScannerState.BlockComment) {
                string text = sourceSnapshotSpan.GetText();
                endOffset = text.IndexOf((char)Delimiter.RightCurlyBracket);
                if (endOffset == -1) {
                    token.Type = new CreationKitDocumentation(text);
                    token.Span = sourceSnapshotSpan;
                }
                else {
                    state = TokenScannerState.Text;
                    length = (endOffset + 1);
                    token.Type = new CreationKitDocumentation(text.Substring(0, length));
                    token.Span = sourceSnapshotSpan.Subspan(0, length);
                }
                return true;
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            int endOffset, length;
            if (state == TokenScannerState.Text) {
                if (sourceTextSpan.FirstOrDefault() == Characters.LeftCurlyBracket) {
                    endOffset = sourceTextSpan.IndexOf(Characters.RightCurlyBracket, 1);
                    if (endOffset == -1) {
                        state = TokenScannerState.Documentation;
                        token = new CreationKitDocumentation(sourceTextSpan);
                    }
                    else {
                        length = (endOffset + 1);
                        token = new CreationKitDocumentation(sourceTextSpan.Substring(0, length));
                    }
                    return true;
                }
            }
            else if (state == TokenScannerState.BlockComment) {
                endOffset = sourceTextSpan.IndexOf(Characters.RightCurlyBracket);
                if (endOffset == -1) {
                    token = new CreationKitDocumentation(sourceTextSpan);
                }
                else {
                    state = TokenScannerState.Text;
                    length = (endOffset + 1);
                    token = new CreationKitDocumentation(sourceTextSpan.Substring(0, length));
                }
                return true;
            }
            token = null;
            return false;
        }
    }
}
