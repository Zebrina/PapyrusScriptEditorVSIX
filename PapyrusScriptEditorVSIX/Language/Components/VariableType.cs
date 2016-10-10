using Papyrus.Language.Components.Tokens;
using Papyrus.Language.Exceptions;
using System;
using System.Collections.Generic;

namespace Papyrus.Language.Components {
    public struct VariableType : ISyntaxParsable {
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

        public override string ToString() {
            if (IsArray) {
                return String.Concat(Type, "[]");
            }
            return (string)Type;
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            int remainingCount = tokens.Count - offset;
            if (remainingCount >= 1 && tokens[offset].IsVariableType) {
                Type = tokens[offset];
                if (remainingCount >= 3 && tokens[offset + 1] == Delimiter.LeftSquareBracket && tokens[offset + 2] == Delimiter.RightSquareBracket) {
                    IsArray = true;
                }
                return true;
            }
            return false;
        }
    }
}
