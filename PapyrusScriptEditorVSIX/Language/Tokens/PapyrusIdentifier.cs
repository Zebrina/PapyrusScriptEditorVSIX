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
    public enum IdentifierType {
        Unknown,
        Local,
        Field,
        Property,
        Function,
        Event,
        State,
        Group,
        Struct,
    }

    public sealed class PapyrusIdentifier : IPapyrusToken, ISyntaxColorableToken {
        public PapyrusTokenType Type { get { return PapyrusTokenType.Identifier; } }
        public string Name { get; private set; }
        public int TokenSize { get { return Name.Length; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusIdentifier(string name) {
            this.Name = name;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusIdentierColorFormat.Name);
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
                return Char.IsLetterOrDigit(c) || c == '_';
            });
        }
    }

    internal class PapyrusIdentifierParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            token = null;

            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                int length = PapyrusDelimiter.FindNext(context.Source, 0);
                if (PapyrusIdentifier.IsValid(context.Source.Substring(0, length))) {
                    token = new PapyrusIdentifier(context.Source.Substring(0, length));
                }
            }

            return token != null;
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
