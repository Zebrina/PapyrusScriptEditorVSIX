using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Language_NEW.Parsing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW.Parsing {
    public enum TokenScannerState {
        Default,
        Comment,
        Documentation,
    }

    public class TokenScanner {
        private List<ITokenParser> parsers = new List<ITokenParser>();

        public TokenScannerState CurrentState { get; set; }

        public TokenScanner() { }

        public void AddParser(ITokenParser parser) {
            parsers.Add(parser);
        }

        private bool TryParse(ITokenParser parser, SnapshotSpan sourceSnapshotSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, PapyrusTokenInfo tokenInfo) {
            IPapyrusToken token;
            if (parser.TryParse(sourceSnapshotSpan.GetText(), scanner, previousTokens, out token)) {
                tokenInfo.Type = token;
                tokenInfo.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, token.TokenSize));
                return true;
            }
            return false;
        }
    }
}
