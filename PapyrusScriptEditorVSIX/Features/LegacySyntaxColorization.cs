using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;


namespace Papyrus.Features {
    public enum TokenColorID {
        Text,
        Comment,
        CreationKitInfo,
        String,
        Literal,
        Keyword,
        ScriptObject,
        Identifier,
    }
    
    public static class LegacySyntaxColorization {
        public static IReadOnlyDictionary<TokenColorID, IVsColorableItem> ColorableItems { get { return colorableItems; } }
        private static Dictionary<TokenColorID, IVsColorableItem> colorableItems = new Dictionary<TokenColorID, IVsColorableItem>();

        internal static void AddColorableItem(TokenColorID tokenColor, string name) {
            colorableItems.Add(tokenColor, new ColorableItem(name, name, COLORINDEX.CI_SYSTEXT_FG, COLORINDEX.CI_SYSTEXT_BK, System.Drawing.Color.Empty, System.Drawing.Color.Empty, FONTFLAGS.FF_DEFAULT));
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyText : ClassificationFormatDefinition {
        public const string Name = "Papyrus Text";

        public PapyrusColorPropertyText() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(218, 218, 218);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.Text, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyComment : ClassificationFormatDefinition {
        public const string Name = "Papyrus Comment";

        public PapyrusColorPropertyComment() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(87, 166, 74);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.Comment, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyDocumentation : ClassificationFormatDefinition {
        public const string Name = "Papyrus Creation Kit Documentation";

        public PapyrusColorPropertyDocumentation() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(96, 139, 78);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.CreationKitInfo, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyStringLiteral : ClassificationFormatDefinition {
        public const string Name = "Papyrus String";

        public PapyrusColorPropertyStringLiteral() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(214, 157, 133);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.String, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyLiteral : ClassificationFormatDefinition {
        public const string Name = "Papyrus Literal";

        public PapyrusColorPropertyLiteral() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(181, 206, 168);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.Literal, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyKeyword : ClassificationFormatDefinition {
        public const string Name = "Papyrus Keyword";

        public PapyrusColorPropertyKeyword() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(86, 156, 214);
            IsBold = true;

            LegacySyntaxColorization.AddColorableItem(TokenColorID.Keyword, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyScriptObject : ClassificationFormatDefinition {
        public const string Name = "Papyrus Script Object";

        public PapyrusColorPropertyScriptObject() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(78, 201, 176);
            IsBold = true;

            LegacySyntaxColorization.AddColorableItem(TokenColorID.ScriptObject, Name);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    public sealed class PapyrusColorPropertyIdentifier : ClassificationFormatDefinition {
        public const string Name = "Papyrus Identifier";

        public PapyrusColorPropertyIdentifier() {
            DisplayName = Name;
            ForegroundColor = Color.FromRgb(220, 220, 220);

            LegacySyntaxColorization.AddColorableItem(TokenColorID.Identifier, Name);
        }
    }
}