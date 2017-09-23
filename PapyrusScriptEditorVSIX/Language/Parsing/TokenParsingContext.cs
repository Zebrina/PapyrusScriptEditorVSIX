using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Parsing {
    public class TokenParsingContext {
        public TokenScanner Scanner { get; set; }
        public string Source { get; set; }
        public IEnumerable<IPapyrusToken> PreviousTokens { get; set; }

        public IReadOnlyTokenSnapshot ParsedSnapshot { get; set; }
        public IReadOnlyTokenSnapshotLine ParsedSnapshotLine { get; set; }
    }
}
