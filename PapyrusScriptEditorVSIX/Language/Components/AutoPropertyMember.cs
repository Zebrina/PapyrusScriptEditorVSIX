using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    //[DebuggerStepThrough]
    public sealed class AutoPropertyMember : IScriptMember {
        private TokenType type;
        private string name;
        private TokenType defaultValue;
        private bool readOnly;
        private bool conditional;
        private bool hidden;

        public AutoPropertyMember(TokenType type, string name, TokenType defaultValue, bool readOnly = true, bool conditional = false, bool hidden = false) {
            this.type = type;
            this.name = name;
            this.defaultValue = defaultValue;
            this.readOnly = readOnly;
            this.conditional = conditional;
            this.hidden = hidden;
        }
        public AutoPropertyMember(TokenType type, string name, bool readOnly = true, bool conditional = false, bool hidden = false) :
            this(type, name, null, readOnly, conditional, hidden) {
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

            b.Append(' ');
            b.Append(readOnly ? Keyword.AutoReadOnly : Keyword.Auto);

            if (conditional) {
                b.Append(' ');
                b.Append(Keyword.Conditional);
            }
            if (hidden) {
                b.Append(' ');
                b.Append(Keyword.Hidden);
            }

            return b.ToString();
        }
        public string GetDefinition() {
            return "";
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

        public override string ToString() {
            return GetDeclaration();
        }
    }
}
