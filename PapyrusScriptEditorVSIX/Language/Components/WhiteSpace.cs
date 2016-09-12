using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    [DebuggerStepThrough]
    public class WhiteSpace : TokenType {
        private string value;

        public WhiteSpace(string value) {
            this.value = value;
        }

        public override string Text {
            get { return value; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.WhiteSpace; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Text; }
        }

        public override bool IgnoredInLine {
            get { return true; }
        }

        public override int GetHashCode() {
            return value.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is WhiteSpace && String.Equals(this.value, ((WhiteSpace)obj).value);
        }
    }
}
