using Papyrus.Language.Components.Tokens;
using Papyrus.Language.Exceptions;
using System;
using System.Collections.Generic;

namespace Papyrus.Language.Components {
    public struct VariableDefaultValue {
        public Token DefaultValue { get; private set; }

        public VariableDefaultValue(Token defaultValue) {
            if (!defaultValue.IsCompileTimeConstant) {
                throw new TokenArgumentException(TokenArgumentExceptionType.NotCompileTimeConstant, defaultValue, "defaultValue");
            }

            this.DefaultValue = defaultValue;
        }

        public bool IsValid {
            get { return DefaultValue != null; }
        }

        public int Length {
            get { return DefaultValue == null ? 0 : 2; }
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            if (tokens.Count - offset >= 2 && tokens[offset] == Operator.BasicAssignment && tokens[offset + 1].IsCompileTimeConstant) {
                DefaultValue = tokens[offset + 1];
            }
            return true;
        }

        public override string ToString() {
            if (IsValid) {
                return String.Concat(" = ", DefaultValue);
            }
            return String.Empty;
        }

        public static implicit operator VariableDefaultValue(Token token) {
            return new VariableDefaultValue(token);
        }
    }
}
