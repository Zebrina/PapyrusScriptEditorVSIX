using Papyrus.Language.Components;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public interface IComponentParser {
        bool TryParse(IReadOnlyTokenSnapshotLine line, ComponentScanner scanner, out IScriptObjectComponent component);
    }

    public class ComponentScanner {
        public StateMember CurrentState { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }
    }
}
