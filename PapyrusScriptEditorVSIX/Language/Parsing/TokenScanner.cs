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

        public int ScanLine(ITextSnapshotLine source, ICollection<PapyrusTokenInfo> result) {
            int initialTokenCount = result.Count;

            TokenParsingContext context = new TokenParsingContext();
            context.Scanner = this;
            context.Source = source.GetText();
            
            int offset = 0;
            while (offset < source.Length) {
                // Update previous tokens.
                context.PreviousTokens = result.Select(t => t.Type);

                PapyrusTokenInfo tokenInfo = new PapyrusTokenInfo();
                foreach (ITokenParser parser in parsers) {
                    IPapyrusToken token;
                    if (parser.TryParse(context, out token)) {
                        tokenInfo.Type = token;
                        tokenInfo.Span = new SnapshotSpan(source.Snapshot, source.Extent.Subspan(offset, token.TokenSize));
                        result.Add(tokenInfo);

                        offset += token.TokenSize;

                        break;
                    }
                }

                // Update source.
                context.Source = source.GetText().Substring(offset);
            }

            return result.Count - initialTokenCount;
        }

        public int ScanLine(string sourceLine, ICollection<IPapyrusToken> result) {
            int initialTokenCount = result.Count;

            TokenParsingContext context = new TokenParsingContext();
            context.Scanner = this;
            context.Source = sourceLine;
            context.PreviousTokens = result;

            int offset = 0;
            while (offset < sourceLine.Length) {
                foreach (ITokenParser parser in parsers) {
                    IPapyrusToken token;
                    if (parser.TryParse(context, out token)) {
                        result.Add(token);

                        offset += token.TokenSize;

                        break;
                    }
                }

                // Update source.
                context.Source = sourceLine.Substring(offset);
            }

            return result.Count - initialTokenCount;
        }
    }
}
