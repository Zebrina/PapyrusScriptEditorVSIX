using Microsoft.VisualStudio.Package;
using Papyrus.Language.Parsing;
using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using Papyrus.Features;

namespace Papyrus.Language.Components {
    public class Parameter : TokenType {
        private TokenType type { get; set; }
        private string name;
        private TokenType defaultValue { get; set; }

        public Parameter(TokenType type, string name, TokenType defaultValue) {
            Debug.Assert(type.VariableType);
            Debug.Assert(defaultValue == null || defaultValue.CompileTimeConstant);

            this.type = type;
            this.name = name;
            this.defaultValue = defaultValue;
        }
        public Parameter(TokenType type, string name) :
            this(type, name, null) {
        }


        public override string Text {
            get { return String.Empty; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.OtherOrInvalid; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Text; }
        }
        public override int Size {
            get {
                int size = type.Size + 1;
                if (!IsNullOrInvalid(defaultValue)) {
                    size += (1 + defaultValue.Size);
                }
                return size;
            }
        }

        public override bool IgnoredInLine {
            get { return true; }
        }

        public override IReadOnlyCollection<IScriptMember> GetMemberList() {
            return type.GetMemberList();
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Parameter && this.type == ((Parameter)obj).type;
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            b.Append(type.Text);
            b.Append(' ');
            b.Append(name);

            if (!IsNullOrInvalid(defaultValue)) {
                b.Append(" = ");
                b.Append(defaultValue);
            }

            return b.ToString();
        }
    }
}
