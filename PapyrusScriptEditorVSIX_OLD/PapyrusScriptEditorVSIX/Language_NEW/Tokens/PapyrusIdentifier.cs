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
    public class PapyrusIdentifier : ISyntaxColorableToken {

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusIdentierColorFormat.Name);
        }
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PapyrusIdentierColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusIdentifier";

        internal PapyrusIdentierColorFormat() {
            DisplayName = "Papyrus Identifier";
            ForegroundColor = Color.FromRgb(220, 220, 220);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusIdentifierColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusIdentierColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
