using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
using System.Collections.Generic;
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
    internal sealed class IdentifierColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusIdentifier";

        internal IdentifierColorFormat() {
            DisplayName = "Papyrus Identifier";
            ForegroundColor = Color.FromRgb(220, 220, 220);
        }
    }

    [DebuggerStepThrough]
    internal static class IdentifierColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(IdentifierColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

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

    [DebuggerStepThrough]
    public sealed class Identifier : Token, ISyntaxColorable {
        private static TokenManager<Identifier> Manager = new TokenManager<Identifier>(false);

        public IdentifierType type;
        private string name;

        public Identifier(IdentifierType type, string name) {
            this.type = type;
            this.name = name;

        }
        public Identifier(string name) :
            this(IdentifierType.Unknown, name) {
        }

        public override string Text {
            get { return name; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Identifier; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(IdentifierColorFormat.Name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Identifier && String.Equals(this.name, ((Identifier)obj).name, StringComparison.OrdinalIgnoreCase);
        }

        public static explicit operator string(Identifier identifier) {
            return identifier.name;
        }
        public static implicit operator Identifier(string value) {
            return new Identifier(value);
        }

        public static bool operator ==(Identifier x, Identifier y) {
            return Equals(x, y);
        }
        public static bool operator !=(Identifier x, Identifier y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Identifier x, Token y) {
            return x == y as Identifier;
        }
        public static bool operator !=(Identifier x, Token y) {
            return x == y as Identifier;
        }
        public static bool operator ==(Token x, Identifier y) {
            return x as Identifier == y;
        }
        public static bool operator !=(Token x, Identifier y) {
            return x as Identifier != y;
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
                return Char.IsLetterOrDigit(c) ||c == '_';
            });
        }
    }

    internal class IdentifierParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<Token> previousTokens, out Token token) {
            token = null;

            if (scanner.TopLevelState == ScannerStates.GlobalScope) {
                int length = Delimiter.FindNext(sourceTextSpan, 0);
                if (Identifier.IsValid(sourceTextSpan.Substring(0, length))) {
                    string name = sourceTextSpan.Substring(0, length);

                    Token lastToken = previousTokens.LastOrDefault();
                    if (lastToken != null && lastToken.IsVariableType) {
                        token = new Identifier(IdentifierType.Field, name);
                    }
                    else if (lastToken == Keyword.Property) {
                        token = new Identifier(IdentifierType.Property, name);
                    }
                    else if (lastToken == Keyword.Function) {
                        token = new Identifier(IdentifierType.Function, name);
                    }
                    else if (lastToken == Keyword.Event) {
                        token = new Identifier(IdentifierType.Event, name);
                    }
                    else {
                        token = new Identifier(name);
                    }
                }
            }

            return token != null;
        }
    }
}
