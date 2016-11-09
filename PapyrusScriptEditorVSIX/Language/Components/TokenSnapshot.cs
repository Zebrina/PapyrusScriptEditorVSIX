using Microsoft.VisualStudio.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Papyrus.Language.Components {
    public interface IReadOnlyTokenSnapshot : IReadOnlyCollection<TokenSnapshotLine> {
        ITextSnapshot BaseTextSnapshot { get; }
        PapyrusTokenInfo this[int line, int position] { get; }
        IEnumerable<PapyrusTokenInfo> Tokens { get; }
        IEnumerable<PapyrusTokenInfo> ParseableTokens { get; }
        bool IsEmpty { get; }
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

        private List<TokenSnapshotLine> container;

        public ITextSnapshot BaseTextSnapshot { get; private set; }

        public TokenSnapshot(ITextSnapshot baseTextSnapshot) {
            if (baseTextSnapshot == null) throw new ArgumentNullException("baseTextSnapshot");

            this.container = new List<TokenSnapshotLine>();
            this.BaseTextSnapshot = baseTextSnapshot;
        }

        public TokenSnapshotLine this[int position] {
            get { return container[position]; }
        }
        public PapyrusTokenInfo this[int line, int position] {
            get { return container[line][position]; }
        }
        public int Count {
            get { return container.Count; }
        }
        public bool IsEmpty {
            get { return container.Count == 0; }
        }
        bool ICollection<TokenSnapshotLine>.IsReadOnly {
            get { return ((ICollection<TokenSnapshotLine>)container).IsReadOnly; }
        }

        public IEnumerable<PapyrusTokenInfo> Tokens {
            get {
                foreach (var line in container) {
                    foreach (var token in line) {
                        yield return token;
                    }
                }
            }
        }
        public IEnumerable<PapyrusTokenInfo> ParseableTokens {
            get {
                foreach (var line in container) {
                    foreach (var token in line.ParseableTokens) {
                        yield return token;
                    }
                }
            }
        }

        public void Add(TokenSnapshotLine item) {
            if (item != null && item.Count > 0) {
                container.Add(item);
            }
        }

        public bool Contains(TokenSnapshotLine item) {
            return container.Contains(item);
        }

        public bool IndexOfToken(Predicate<PapyrusTokenInfo> pred, out int lineNumber, out int linePosition) {
            for (int i = 0; i < container.Count; ++i) {
                TokenSnapshotLine snapshotLine = container[i];
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
            container.CopyTo(array, arrayIndex);
        }

        bool ICollection<TokenSnapshotLine>.Remove(TokenSnapshotLine item) {
            return container.Remove(item);
        }

        public void Clear() {
            container.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return container.GetEnumerator();
        }
        public IEnumerator<TokenSnapshotLine> GetEnumerator() {
            return container.GetEnumerator();
        }
    }
}
