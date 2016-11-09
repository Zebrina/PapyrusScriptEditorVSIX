using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language {
    public class TokenParserInfo {
        public TokenScannerState ParseState { get; set; }
        public ScannerSyntaxState SyntaxState { get; set; }
        public IEnumerable<Token> PreviouslyParsedTokens { get; private set; }
    }

    //[DebuggerStepThrough]
    public abstract class TokenParser {
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, PapyrusTokenInfo tokenInfo) {
            Token token;
            if (TryParse(sourceSnapshotSpan.GetText(), ref state, previousTokens, out token)) {
                tokenInfo.Type = token;
                tokenInfo.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, token.Text.Length));
                return true;
            }
            return false;
        }
        public abstract bool TryParse(string sourceTextSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token);
    }

    public enum TokenScannerState {
        Text,
        BlockComment,
        Documentation,
        ParameterList,
        PropertyGetScope,
        PropertySetScope,
        FunctionScope,
        EventScope,
        GroupScope,
        StructScope,
    }

    public enum ScannerSyntaxState {

    }

    //[DebuggerStepThrough]
    public class TokenScanner {
        private ICollection<TokenParser> handlers = new List<TokenParser>();
        private TokenScannerState state = TokenScannerState.Text;

        public void ForceState(TokenScannerState state) {
            this.state = state;
        }
        public void Reset() {
            state = TokenScannerState.Text;
        }

        public static TokenScanner operator +(TokenScanner scanner, TokenParser matcher) {
            if (scanner != null) {
                scanner.handlers.Add(matcher);
            }
            return scanner;
        }

        private bool Scan(SnapshotSpan span, ref TokenScannerState state, IEnumerable<Token> tokenCollection, PapyrusTokenInfo token) {
            if (!span.IsEmpty) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(span, ref state, tokenCollection, token)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ScanSpan(SnapshotSpan span, ICollection<PapyrusTokenInfo> tokenCollection) {
            while (!span.IsEmpty) {
                PapyrusTokenInfo token = new PapyrusTokenInfo();
                if (!Scan(span.Ignore(), ref state, tokenCollection.Select(t => t.Type), token)) {
                    return;
                }
                tokenCollection.Add(token);
                span = span.Subspan(token.Span.End);
            }
        }
        public void ScanLine(ITextSnapshotLine textLine, ICollection<PapyrusTokenInfo> tokenCollection) {
            ScanSpan(textLine.Extent, tokenCollection);
        }

        private bool Scan(string textSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token) {
            if (!String.IsNullOrWhiteSpace(textSpan)) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(textSpan, ref state, previousTokens, out token)) {
                        return true;
                    }
                }
            }
            token = null;
            return false;
        }
        public void ScanSpan(string textSpan, ICollection<Token> tokenCollection) {
            while (!String.IsNullOrWhiteSpace(textSpan)) {
                Token token;
                textSpan = textSpan.TrimStart('\r', '\n', '\t', ' ');
                if (!Scan(textSpan, ref state, tokenCollection, out token)) {
                    return;
                }
                tokenCollection.Add(token);
                textSpan = textSpan.Remove(0, token.Text.Length);
            }
        }
        public void ScanLine(string textLine, ICollection<Token> tokenCollection) {
            ScanSpan(textLine, tokenCollection);
        }

        public static TokenScanner IncludeAllParsers() {
            TokenScanner scanner = new TokenScanner();
            scanner += new BlockCommentParser();
            scanner += new CreationKitDocumentationParser();
            scanner += new LineCommentParser();
            scanner += new StringLiteralParser();
            scanner += new NumericLiteralParser();
            scanner += new OperatorParser();
            scanner += new DelimiterParser();
            scanner += new KeywordParser();
            scanner += new ScriptObjectParser();
            scanner += new IdentifierParser();
            return scanner;
        }
    }
}
