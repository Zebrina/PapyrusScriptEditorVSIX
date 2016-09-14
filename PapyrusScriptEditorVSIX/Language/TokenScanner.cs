using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Language.Components;
using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Papyrus.Language {
    public abstract class TokenScannerModule {
        private TokenScannerModule next = null;

        protected abstract bool Scan(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, out Token token);
        public bool Scan(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, out Token token) {
            return Scan(sourceSnapshotSpan, ref state, out token) ||
                (next != null && next.Scan(sourceSnapshotSpan, ref state, out token));
        }

        private void Add(TokenScannerModule module) {
            if (module != null) {
                if (next == null) {
                    next = module;
                }
                else {
                    next.Add(module);
                }
            }
        }

        public static TokenScannerModule operator +(TokenScannerModule x, TokenScannerModule y) {
            if (x == null) {
                return y;
            }
            x.Add(y);
            return x;
        }
    }

    public enum TokenScannerState {
        Text,
        BlockComment,
        Documentation,
        ParameterList,
    }

    public class TokenScanner {
        private TokenScannerModule tokenScannerModule;
        private TokenScannerState state;

        public TokenScanner(TokenScannerModule tokenScannerModule) {
            this.tokenScannerModule = tokenScannerModule;
            this.state = TokenScannerState.Text;
        }

        public void ForceState(TokenScannerState state) {
            this.state = state;
        }

        public bool ScanLine(ITextSnapshotLine textLine, TokenSnapshotLine tokenLine) {
            Token token;
            SnapshotSpan span = textLine.ToSpan();
            bool eol = false;
            while (!eol) {
                span = span.IgnoreWhile();
                eol = tokenScannerModule.Scan(span, offset, ref state, out token);
                tokenLine.AddToken(token);
                span = span.Subspan(token.Span.Length);
            }
            return !tokenLine.IsEmpty;
        }
        public bool ScanSource(ITextSnapshot textSource, TokenSnapshot tokenSource) {
            bool success = true;
            foreach (var lineIn in textSource.Lines) {
                TokenSnapshotLine tokenLine = new TokenSnapshotLine();
                if (ScanLine(lineIn, tokenLine)) {
                    tokenSource.AddLine(tokenLine);
                }
                else {
                    success = false;
                }
            }
            return success;
        }
    }
}
