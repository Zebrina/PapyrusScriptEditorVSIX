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

        public abstract bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token);
        public bool ScanLine(ITextSnapshotLine sourceSnapshotLine, int offset, ref TokenScannerState state, out Token token) {
            return Scan(new SnapshotSpan(sourceSnapshotLine.Start, sourceSnapshotLine.End), offset, ref state, out token);
        }

        /*
        public bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, ICollection<Token> tokens) {
            Token token;
            if (Scan(sourceSnapshotSpan, offset, ref state, out token)) {
                tokens.Add(token);
                return true;
            }
            return next != null && next.Scan(sourceSnapshotSpan, offset, ref state, out token);
        }
        public bool ScanLine(ITextSnapshotLine sourceSnapshotSpan, int offset, ref TokenScannerState state, ICollection<Token> tokens) {
            Token token;
            if (ScanLine(sourceSnapshotSpan, offset, ref state, out token)) {
                tokens.Add(token);
                return true;
            }
            return next != null && next.ScanLine(sourceSnapshotSpan, offset, ref state, out token);
        }
        */

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

    public enum TokenScannerResult {
        EndSource,
        EndLine,
        ExtendedLine,
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
        private int offset;

        public TokenScanner(TokenScannerModule tokenScannerModule) {
            this.tokenScannerModule = tokenScannerModule;
            this.state = TokenScannerState.Text;
            this.offset = 0;
        }

        public void ForceState(TokenScannerState state) {
            this.state = state;
        }

        public void ScanSnapshotLine(ITextSnapshotLine snapshotLine, IList<Token> tokens) {
            offset = 0;
            Token token;
            while (tokenScannerModule.ScanLine(snapshotLine, offset, ref state, out token)) {
                tokens.Add(token);
                offset += token.Span.Length;
            }
        }

        public IList<IList<Token>> ScanSnapshot(ITextSnapshot snapshot) {
            state = TokenScannerState.Text;
            var result = new List<IList<Token>>();
            foreach (ITextSnapshotLine line in snapshot.Lines) {
                var tokens = new List<Token>();
                ScanSnapshotLine(line, tokens);

                if (tokens.Count > 0) {
                    result.Add(tokens);
                }
            }

            return result;
        }
    }
}
