using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Papyrus.Language {
    //[DebuggerStepThrough]
    public abstract class TokenParser {
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo tokenInfo) {
            Token token;
            if (TryParse(sourceSnapshotSpan.GetText(), ref state, out token)) {
                tokenInfo.Type = token;
                tokenInfo.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, token.Text.Length));
                return true;
            }
            return false;
        }
        public abstract bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token);
    }

    public enum TokenScannerState {
        Text,
        BlockComment,
        Documentation,
        ParameterList,
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

        private bool Scan(SnapshotSpan span, ref TokenScannerState state, TokenInfo token) {
            if (!span.IsEmpty) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(span, ref state, token)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ScanSpan(SnapshotSpan span, ICollection<TokenInfo> tokenCollection) {
            while (!span.IsEmpty) {
                TokenInfo token = new TokenInfo();
                if (!Scan(span.Ignore(), ref state, token)) {
                    return;
                }
                tokenCollection.Add(token);
                span = span.Subspan(token.Span.End);
            }
        }
        public void ScanLine(ITextSnapshotLine textLine, ICollection<TokenInfo> tokenCollection) {
            ScanSpan(textLine.Extent, tokenCollection);
        }

        private bool Scan(string textSpan, ref TokenScannerState state, out Token token) {
            if (!String.IsNullOrWhiteSpace(textSpan)) {
                foreach (var handler in handlers) {
                    if (handler.TryParse(textSpan, ref state, out token)) {
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
                if (!Scan(textSpan, ref state, out token)) {
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
