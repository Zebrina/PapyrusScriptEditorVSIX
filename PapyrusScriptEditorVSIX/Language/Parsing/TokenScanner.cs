using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Parsing {
    public interface ITokenParser {
        bool TryParse(TokenParsingContext context, out IPapyrusToken token);
    }

    public enum TokenScannerState {
        Default,
        Comment,
        Documentation,
    }

    public class TokenScanner {
        private List<ITokenParser> parsers = new List<ITokenParser>();

        private Stack<TokenScannerState> states = new Stack<TokenScannerState>();
        public TokenScannerState CurrentState {
            get {
                if (states.Count == 0) {
                    return TokenScannerState.Default;
                }
                return states.Peek();
            }
        }

        public void AddParser(ITokenParser tokenParser) {
            parsers.Add(tokenParser);
        }

        public void GoToState(TokenScannerState newState) {
            states.Push(newState);
        }
        public void GoToPreviousState() {
            if (states.Count > 0) {
                states.Pop();
            }
        }

        private bool TryParse(ITokenParser parser, SnapshotSpan sourceSnapshotSpan, IEnumerable<IPapyrusToken> previousTokens, PapyrusTokenInfo tokenInfo) {
            IPapyrusToken token;
            TokenParsingContext context = new TokenParsingContext();
            context.Scanner = this;
            context.Source = sourceSnapshotSpan.GetText();
            context.PreviousTokens = previousTokens;
            //context.Scanner = this;
            //context.Source = sourceSnapshotSpan.GetText();

            if (parser.TryParse(context, out token)) {
                tokenInfo.Type = token;
                tokenInfo.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, token.TokenSize));
                return true;
            }
            return false;
        }

        public void ScanLine(ITextSnapshotLine source, ICollection<PapyrusTokenInfo> result) {
            SnapshotSpan sourceSnapshotSpan = source.Extent;
            while (!sourceSnapshotSpan.IsEmpty) {
                PapyrusTokenInfo tokenInfo = new PapyrusTokenInfo();
                foreach (ITokenParser parser in parsers) {
                    if (TryParse(parser, sourceSnapshotSpan, result.Select(t => t.Type), tokenInfo)) {
                        result.Add(tokenInfo);
                        if (tokenInfo.Span.End.Position > sourceSnapshotSpan.End.Position) {
                            return;
                        }
                        sourceSnapshotSpan = sourceSnapshotSpan.Subspan(tokenInfo.Span.Length);
                        break;
                    }
                }
            }
        }
    }
}
