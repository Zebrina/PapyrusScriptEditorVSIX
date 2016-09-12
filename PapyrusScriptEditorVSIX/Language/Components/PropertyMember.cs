using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public sealed class PropertyMember : IScriptMember {
        private TokenType type;
        private string name;
        private bool hidden;
        private FunctionMember getMethod;
        private FunctionMember setMethod;

        public PropertyMember(TokenType type, string name, FunctionMember getMethod, FunctionMember setMethod, bool hidden = false) {
            this.type = type;
            this.name = name;
            this.hidden = hidden;
            this.getMethod = getMethod;
            this.setMethod = setMethod;
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            b.Append(type.Text);
            b.Append(' ');
            b.Append(Keyword.Property);
            b.Append(' ');
            b.Append(name);

            if (hidden) {
                b.Append(' ');
                b.Append(Keyword.Hidden);
            }

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
        bool IScriptMember.ChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return new Parameter[0]; }
        }
        bool IScriptMember.Hidden {
            get { return hidden; }
        }

        int IComparable<IScriptMember>.CompareTo(IScriptMember other) {
            return String.Compare(this.name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
