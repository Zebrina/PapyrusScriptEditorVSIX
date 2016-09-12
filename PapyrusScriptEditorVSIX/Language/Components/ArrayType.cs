using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    [DebuggerStepThrough]
    public class ArrayType : TokenType {
        private static AutoPropertyMember Length = new AutoPropertyMember(Keyword.Int, Keyword.Length);

        private TokenType type;

        public ArrayType(TokenType type) {
            this.type = type;
        }

        public override string Text {
            get { return String.Concat(type, Delimiter.LeftSquareBracket, Delimiter.RightSquareBracket); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.OtherOrInvalid; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Text; }
        }
        public override int Size {
            get { return 3; }
        }

        public override bool VariableType {
            get { return true; }
        }

        public override bool IgnoredInLine {
            get { return true; }
        }

        public override IReadOnlyCollection<IScriptMember> GetMemberList() {
            return new IScriptMember[] { Length };
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is ArrayType && this.type == ((ArrayType)obj).type;
        }
    }
}
