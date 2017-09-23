using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public class PapyrusField {
        public VariableType Type { get; private set; }
        public string Name { get; private set; }

        /*
        bool IScriptObjectComponent.IsSubComponent {
            get { return true; }
        }
        int IScriptObjectComponent.NumTokens() {
            return ((IScriptObjectComponent)Type).NumTokens() + 1;
        }
        */

        public PapyrusField(VariableType type, string name) {
#warning TODO: Needs to throw exception for void types.
            this.Type = type;
            this.Name = name;
        }
    }
}
