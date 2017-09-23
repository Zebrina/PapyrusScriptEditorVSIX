using Papyrus.Language.Components.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public class PapyrusStruct : IScriptObjectComponent, IEnumerable<PapyrusField>, IReadOnlyCollection<PapyrusField> {
        private static PapyrusStruct instance = new PapyrusStruct();

        private readonly List<PapyrusField> memberList = new List<PapyrusField>();

        public int Count {
            get { return memberList.Count; }
        }

        bool IScriptObjectComponent.IsSubComponent {
            get { return false; }
        }
        int IScriptObjectComponent.NumTokens() {
            return memberList.Sum(c => ((IScriptObjectComponent)c).NumTokens());
        }
        bool IScriptObjectComponent.TryParse(IEnumerable<Token> tokens) {
            return false;
        }

        public IEnumerator<PapyrusField> GetEnumerator() {
            return memberList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return memberList.GetEnumerator();
        }
    }
}
