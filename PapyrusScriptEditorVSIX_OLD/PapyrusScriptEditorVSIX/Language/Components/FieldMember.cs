#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public sealed class FieldMember : IScriptMember, IComparable<IScriptMember> {
        public string Name {
            get { throw new NotImplementedException(); }
        }

        public string GetDeclaration() {
            throw new NotImplementedException();
        }
        public string GetDefinition() {
            return String.Empty;
        }

        public IEnumerable<Parameter> Parameters {
            get {
                throw new NotImplementedException();
            }
        }

        public bool ChildAccessible {
            get { return false; }
        }
        public bool Hidden {
            get { throw new NotImplementedException(); }
        }

        int IComparable<IScriptMember>.CompareTo(IScriptMember other) {
            return String.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}

#endif