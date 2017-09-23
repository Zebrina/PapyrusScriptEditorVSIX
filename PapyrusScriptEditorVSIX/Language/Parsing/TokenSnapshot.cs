using Microsoft.VisualStudio.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Parsing {
    public interface IReadOnlyTokenSnapshot : IReadOnlyCollection<TokenSnapshotLine> {
        ITextSnapshot BaseTextSnapshot { get; }
        PapyrusTokenInfo this[int line, int position] { get; }
        IEnumerable<PapyrusTokenInfo> Tokens { get; }
        IEnumerable<PapyrusTokenInfo> ParseableTokens { get; }
        bool IndexOfToken(Predicate<PapyrusTokenInfo> pred, out int lineNumber, out int linePosition);
    }

    //[DebuggerStepThrough]
    public sealed class TokenSnapshot : IReadOnlyTokenSnapshot, IReadOnlyCollection<TokenSnapshotLine>, ICollection<TokenSnapshotLine>, IEnumerable<TokenSnapshotLine>, IEnumerable {
        /*
        private class KeyComparer : IComparer<ITextSnapshotLine> {
            int IComparer<ITextSnapshotLine>.Compare(ITextSnapshotLine x, ITextSnapshotLine y) {
                return x.LineNumber.CompareTo(y.LineNumber);
            }
        }
        */

        public ITextSnapshot BaseTextSnapshot { get; private set; }

        private List<TokenSnapshotLine> lines;

        public TokenSnapshot(ITextSnapshot baseTextSnapshot) {
            this.lines = new List<TokenSnapshotLine>();
            this.BaseTextSnapshot = baseTextSnapshot;
        }

        public TokenSnapshotLine this[int position] {
            get { return lines[position]; }
        }
        public PapyrusTokenInfo this[int line, int position] {
            get { return lines[line][position]; }
        }
        public int Count {
            get { return lines.Count; }
        }
        bool ICollection<TokenSnapshotLine>.IsReadOnly {
            get { return ((ICollection<TokenSnapshotLine>)lines).IsReadOnly; }
        }

        public IEnumerable<PapyrusTokenInfo> Tokens {
            get {
                foreach (var line in lines) {
                    foreach (var token in line) {
                        yield return token;
                    }
                }
            }
        }
        public IEnumerable<PapyrusTokenInfo> ParseableTokens {
            get {
                foreach (var line in lines) {
                    foreach (var token in line.Where(t => !t.Type.IsIgnoredByParser)) {
                        yield return token;
                    }
                }
            }
        }

        public void Add(TokenSnapshotLine item) {
            if (item != null && item.Count > 0) {
                lines.Add(item);
            }
        }

        public bool Contains(TokenSnapshotLine item) {
            return lines.Contains(item);
        }

        public bool IndexOfToken(Predicate<PapyrusTokenInfo> pred, out int lineNumber, out int linePosition) {
            for (int i = 0; i < lines.Count; ++i) {
                TokenSnapshotLine snapshotLine = lines[i];
                for (int j = 0; j < snapshotLine.Count; ++j) {
                    if (pred.Invoke(snapshotLine[j])) {
                        lineNumber = i;
                        linePosition = j;
                        return true;
                    }
                }
            }

            lineNumber = -1;
            linePosition = -1;
            return false;
        }

        public void CopyTo(TokenSnapshotLine[] array, int arrayIndex) {
            lines.CopyTo(array, arrayIndex);
        }

        bool ICollection<TokenSnapshotLine>.Remove(TokenSnapshotLine item) {
            return lines.Remove(item);
        }

        public void Clear() {
            lines.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return lines.GetEnumerator();
        }
        public IEnumerator<TokenSnapshotLine> GetEnumerator() {
            return lines.GetEnumerator();
        }
    }
}
