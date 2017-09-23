using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language {
    /*
    public class TokenParserInfo {
        public ScannerStates ParseState { get; set; }
        public ScannerSyntaxState SyntaxState { get; set; }
        public IEnumerable<Token> PreviouslyParsedTokens { get; private set; }
    }
    */

    //[DebuggerStepThrough]
    public abstract class TokenParser {
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, TokenScanner scanner, IEnumerable<Token> previousTokens, PapyrusTokenInfo tokenInfo) {
            Token token;
            if (TryParse(sourceSnapshotSpan.GetText(), scanner, previousTokens, out token)) {
                tokenInfo.Type = token;
                tokenInfo.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, token.Text.Length));
                return true;
            }
            return false;
        }
        public abstract bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<Token> previousTokens, out Token token);
    }

    public enum ScannerSyntaxState {

    }

    //[DebuggerStepThrough]
    public class TokenScanner {
        private ICollection<TokenParser> handlers = new List<TokenParser>();
        private Stack<ScannerStates> state = new Stack<ScannerStates>();

        public ScannerStates TopLevelState {
            get { return state.Count == 0 ? ScannerStates.GlobalScope : state.Peek(); }
        }

        public bool SetState(ScannerStates newState) {
            if (TopLevelState == newState) {
                return false;
            }
            state.Push(newState);
            return true;
        }
        public bool ClearState(ScannerStates oldState) {
            if (TopLevelState != oldState) {
                return false;
            }
            state.Pop();
            return true;
        }

        public static TokenScanner operator +(TokenScanner scanner, TokenParser matcher) {
            if (scanner != null) {
                scanner.handlers.Add(matcher);
            }
            return scanner;
        }

        private bool Scan(SnapshotSpan span, IEnumerable<Token> tokenCollection, PapyrusTokenInfo token) {
            if (!span.IsEmpty) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(span, this, tokenCollection, token)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ScanSpan(SnapshotSpan span, ICollection<PapyrusTokenInfo> tokenCollection) {
            while (!span.IsEmpty) {
                PapyrusTokenInfo token = new PapyrusTokenInfo();
                if (!Scan(span.Ignore(), tokenCollection.Select(t => t.Type), token)) {
                    return;
                }
                tokenCollection.Add(token);
                span = span.Subspan(token.Span.End);
            }
        }
        public void ScanLine(ITextSnapshotLine textLine, ICollection<PapyrusTokenInfo> tokenCollection) {
            ScanSpan(textLine.Extent, tokenCollection);
        }

        private bool Scan(string textSpan, IEnumerable<Token> previousTokens, out Token token) {
            if (!String.IsNullOrWhiteSpace(textSpan)) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(textSpan, this, previousTokens, out token)) {
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
                if (!Scan(textSpan, tokenCollection, out token)) {
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
