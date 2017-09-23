using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusDocumentation : IPapyrusToken, ISyntaxColorableToken, IOutlineableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.CreationKitInfo; } }
        public string DocumentationText { get; private set; }
        public int TokenSize { get { return DocumentationText.Length; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusDocumentation(string documentationText) {
            this.DocumentationText = documentationText;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusDocumentationColorFormat.Name);
        }

        bool IOutlineableToken.IsOutlineableStart(IReadOnlyTokenSnapshotLine line) {
            return DocumentationText.FirstOrDefault() == '{' && DocumentationText.LastOrDefault() != '}';
        }
        bool IOutlineableToken.IsOutlineableEnd(IOutlineableToken startToken) {
            return DocumentationText.LastOrDefault() == '}' && DocumentationText.FirstOrDefault() != '{';
        }
        bool IOutlineableToken.IsImplementation {
            get { return false; }
        }
        string IOutlineableToken.CollapsedText {
            get { return "{ ... }"; }
        }
        bool IOutlineableToken.CollapseFirstLine {
            get { return true; }
        }
    }

    internal sealed class PapyrusDocumentationParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            int endOffset, length;
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                if (context.Source.FirstOrDefault() == '{') {
                    endOffset = context.Source.IndexOf('}', 1);
                    if (endOffset == -1) {
                        context.Scanner.GoToState(TokenScannerState.Documentation);
                        token = new PapyrusDocumentation(context.Source);
                    }
                    else {
                        length = (endOffset + 1);
                        token = new PapyrusDocumentation(context.Source.Substring(0, length));
                    }
                    return true;
                }
            }
            else if (context.Scanner.CurrentState == TokenScannerState.Documentation) {
                endOffset = context.Source.IndexOf('}');
                if (endOffset == -1) {
                    token = new PapyrusDocumentation(context.Source);
                }
                else {
                    context.Scanner.GoToPreviousState();
                    length = (endOffset + 1);
                    token = new PapyrusDocumentation(context.Source.Substring(0, length));
                }
                return true;
            }

            token = null;
            return false;
        }
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PapyrusDocumentationColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusDocumentation";

        internal PapyrusDocumentationColorFormat() {
            DisplayName = "Papyrus Documentation (Creation Kit)";
            ForegroundColor = Color.FromRgb(96, 139, 78);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusDocumentationColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusDocumentationColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
