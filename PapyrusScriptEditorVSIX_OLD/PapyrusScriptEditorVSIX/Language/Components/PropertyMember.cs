using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Papyrus.Language.Components {
    public sealed class PropertyMember : IScriptMember, IComparable<IScriptMember>, ICloneable {
        private VariableType type;
        private string name;
        private VariableDefaultValue defaultValue;
        private ScriptMemberAttributes attributes;
        private FunctionMember getMethod;
        private FunctionMember setMethod;

        /*
        private PropertyMember(VariableType type, string name, VariableDefaultValue defaultValue, FunctionMember getMethod, FunctionMember setMethod, IEnumerable<Keyword> attributes) {
            this.type = type;
            this.name = name;
            this.defaultValue = defaultValue;
            this.attributes.AddRange(attributes);
            this.getMethod = getMethod;
            this.setMethod = setMethod;
        }
        private PropertyMember(VariableType type, string name, VariableDefaultValue defaultValue, FunctionMember getMethod, FunctionMember setMethod, params Keyword[] attributes) :
            this(type, name, defaultValue, getMethod, setMethod, attributes.AsEnumerable()) {
        }
        private PropertyMember(VariableType type, string name, VariableDefaultValue defaultValue, params Keyword[] attributes) :
            this(type, name, defaultValue, null, null, attributes) {
        }
        */
        private PropertyMember(PropertyMember other) {
            this.type = other.type;
            this.name = other.name;
            this.defaultValue = other.defaultValue;
            this.attributes = other.attributes;
            this.getMethod = (FunctionMember)other.getMethod.Clone();
            this.setMethod = (FunctionMember)other.setMethod.Clone();
        }
        private PropertyMember() {
            type = default(VariableType);
            name = String.Empty;
            defaultValue = default(VariableDefaultValue);
            attributes = default(ScriptMemberAttributes);
            getMethod = null;
            setMethod = null;
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            b.Append(type);
            b.Append(' ');
            b.Append(Keyword.Property);
            b.Append(' ');
            b.Append(name);
            b.Append(defaultValue);
            b.Append(attributes);

            return b.ToString();
        }
        public string GetDefinition() {
            if (getMethod != null && setMethod != null) {
                return String.Concat(getMethod.GetDefinition(), Environment.NewLine, setMethod.GetDefinition());
            }
            else if (getMethod != null) {
                return getMethod.GetDefinition();
            }
            else if (setMethod != null) {
                return setMethod.GetDefinition();
            }
            return String.Empty;
        }
        bool IScriptMember.IsChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return new Parameter[0]; }
        }
        bool IScriptMember.Hidden {
            get { return attributes.Contains(Keyword.Hidden); }
        }

        public int Length {
            get { return type.Length + 2 + defaultValue.Length + attributes.Length; }
        }

        /*
        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            if (type.TryParse(tokens, offset)) {
                int remainingCount = tokens.Count - offset;
                if (remainingCount >= 2 && tokens[offset] == Keyword.Property && tokens[offset + 1].TypeID == TokenTypeID.Identifier) {
                    name = tokens[offset + 1].Text;
                    offset += 2;

                    defaultValue.TryParse(tokens, offset);
                    offset += defaultValue.Length;
                    attributes.TryParse(tokens, offset);

                    getMethod = null;
                    setMethod = null;

                    return true;
                }
            }
            
            return false;
        }
        */

        int IComparable<IScriptMember>.CompareTo(IScriptMember other) {
            return String.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public object Clone() {
            return new PropertyMember(this);
        }

        private static PropertyMember prototype = new PropertyMember();
        public static bool TryParse(IReadOnlyList<Token> tokens, int offset, out IScriptMember result) {
            if (prototype.TryParse(tokens, offset)) {
                result = (PropertyMember)prototype.MemberwiseClone();
                return true;
            }
            result = null;
            return false;
        }
    }
}
