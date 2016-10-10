using System.Collections.Generic;
using System.Text;

namespace Papyrus.Language.Components {
    public struct Parameter : ISyntaxParsable {
        public VariableType Type { get; private set; }
        public Identifier Name { get; private set; }
        public VariableDefaultValue DefaultValue { get; private set; }

        public Parameter(VariableType type, Identifier name, VariableDefaultValue defaultValue) {
            this.Type = type;
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
        public Parameter(VariableType type, string name) :
            this(type, name, null) {
        }

        public int Length {
            get { return Type.Length + 1 + DefaultValue.Length; }
        }
        public bool HasDefaultValue {
            get { return DefaultValue.IsValid; }
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            b.Append(Type);
            b.Append(' ');
            b.Append(Name);
            b.Append(DefaultValue);

            return b.ToString();
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            if (Type.TryParse(tokens, offset)) {
                offset += Type.Length;
                int remainingCount = tokens.Count - offset;
                if (remainingCount >= 1 && tokens[offset].TypeID == TokenTypeID.Identifier) {
                    Name = tokens[offset].Text;
                    DefaultValue.TryParse(tokens, offset + 1);
                    return true;
                }
            }
            return false;
        }
    }
}
