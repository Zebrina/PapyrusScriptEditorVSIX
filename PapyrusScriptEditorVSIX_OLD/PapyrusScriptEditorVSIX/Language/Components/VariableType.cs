using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using Papyrus.Language.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Papyrus.Language.Components {
    public struct VariableType : IScriptObjectComponent {
        public Token Type { get; private set; }
        public bool IsArray { get; private set; }

        public VariableType(Token type, bool isArray) {
            if (!type.IsVariableType) {
                throw new TokenArgumentException(TokenArgumentExceptionType.NotVariableType, type, "type");
            }

            this.Type = type;
            this.IsArray = isArray;
        }

        public bool IsVoid {
            get { return Type == null; }
        }

        public int Length {
            get { return Type == null ? 0 : (IsArray ? 3 : 1); }
        }

        bool IScriptObjectComponent.IsSubComponent {
            get { return true; }
        }
        int IScriptObjectComponent.NumTokens() {
            return ((IScriptObjectComponent)Type).NumTokens() + 1;
        }

        public override string ToString() {
            if (IsArray) {
                return String.Concat(Type, "[]");
            }
            return (string)Type;
        }

        private static VariableType parsedInstance = new VariableType();

        public bool TryParse(IEnumerable<Token> tokens) {
            var self = new VariableType(); // Anonymous methods can't access this.
            bool result = tokens.FindPattern(delegate (Token token, int index) {
                switch (index) {
                    case 0:
                        if (token.IsVariableType) {
                            self.Type = token;
                            return IterationResult.Continue;
                        }
                        break;

                    case 1:
                        if (token == Delimiter.LeftSquareBracket) {
                            return IterationResult.Continue;
                        }
                        self.IsArray = false;
                        return IterationResult.Success;

                    case 2:
                        if (token == Delimiter.RightSquareBracket) {
                            self.IsArray = true;
                            return IterationResult.Success;
                        }
                        break;
                }
                return IterationResult.Fail;
            });

            // Copy parsed data to 'this'.
            this = self;
            return result;
        }
    }
}
