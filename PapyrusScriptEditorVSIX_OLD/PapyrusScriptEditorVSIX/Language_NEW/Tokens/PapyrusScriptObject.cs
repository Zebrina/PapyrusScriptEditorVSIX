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
    public class PapyrusScriptObject : ISyntaxColorableToken {

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusScriptObjectColorFormat.Name);
        }
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PapyrusScriptObjectColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusScriptObject";

        internal PapyrusScriptObjectColorFormat() {
            DisplayName = "Papyrus Script Object";
            ForegroundColor = Color.FromRgb(78, 201, 176);
            IsBold = true;
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusScriptObjectColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusScriptObjectColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
