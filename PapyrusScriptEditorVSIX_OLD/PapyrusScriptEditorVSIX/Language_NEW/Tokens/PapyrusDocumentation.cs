using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language_NEW.Data;
using Papyrus.Language_NEW.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Papyrus.Language_NEW.Tokens {
    public class PapyrusDocumentation : IPapyrusToken, ISyntaxColorableToken, IOutlineableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.CreationKitInfo; } }
        public string DocumentationText { get; private set; }
        public int TokenSize { get { return DocumentationText.Length; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public bool IsEqualToToken(IPapyrusToken other) {
            throw new NotImplementedException();
        }
        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo textFormatInfo) {
            stringBuilder.Append(DocumentationText);
            return true;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusDocumentationColorFormat.Name);
        }

        bool IOutlineableToken.IsOutlineableStart(IEnumerable<PapyrusTokenInfo> line) {
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
