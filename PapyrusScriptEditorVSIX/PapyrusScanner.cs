#if false
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Papyrus.Language.Components;
using Papyrus.Language.Parsing;

namespace Papyrus {
    public class PapyrusScanner : IScanner {
        string source;
        int offset;
        IVsTextLines buffer;

        TokenParser parser;

        public PapyrusScanner() : base() {
            parser = new TokenParser();
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state) {
            parser.State = (TokenParserState)state;
            Language.Components.TokenType token;
            if (parser.ParseToken(source, offset, out token)) {
                token.CopyToTokenInfo(tokenInfo, offset);

                offset = tokenInfo.EndIndex + 1;

                state = (int)parser.State;

                return true;
            }

            return false;
        }

        public void SetSource(string source, int offset) {
            this.source = source;
            this.offset = offset;
        }

        public void SetBuffer(IVsTextLines buffer) {
            this.buffer = buffer;
        }
    }
}

#endif