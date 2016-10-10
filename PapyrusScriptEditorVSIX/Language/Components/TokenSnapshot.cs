using Microsoft.VisualStudio.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Papyrus.Language.Components {
    public interface IReadOnlyTokenSnapshot : IReadOnlyCollection<TokenSnapshotLine> {
        IEnumerable<TokenInfo> Tokens { get; }
        IEnumerable<TokenInfo> ParseableTokens { get; }
        bool IsEmpty { get; }
    }

    [DebuggerStepThrough]
    public sealed class TokenSnapshot : IReadOnlyTokenSnapshot, IReadOnlyCollection<TokenSnapshotLine>, ICollection<TokenSnapshotLine>, IEnumerable<TokenSnapshotLine>, IEnumerable {
        private class KeyComparer : IComparer<ITextSnapshotLine> {
            int IComparer<ITextSnapshotLine>.Compare(ITextSnapshotLine x, ITextSnapshotLine y) {
                return x.LineNumber.CompareTo(y.LineNumber);
            }
        }

        private SortedList<ITextSnapshotLine, TokenSnapshotLine> container;

        public TokenSnapshot() {
            container = new SortedList<ITextSnapshotLine, TokenSnapshotLine>(new KeyComparer());
        }

        public TokenSnapshotLine this[int position] {
            get { return container.Values[position]; }
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

        public IEnumerable<TokenInfo> Tokens {
            get {
                foreach (var line in container.Values) {
                    foreach (var token in line) {
                        yield return token;
                    }
                }
            }
        }
        public IEnumerable<TokenInfo> ParseableTokens {
            get {
                foreach (var line in container.Values) {
                    foreach (var token in line.ParseableTokens) {
                        yield return token;
                    }
                }
            }
        }

        public void Add(TokenSnapshotLine item) {
            if (item != null && item.BaseTextSnapshotLine != null && item.Count > 0) {
                container.Add(item.BaseTextSnapshotLine, item);
            }
        }

        public bool Contains(TokenSnapshotLine item) {
            return container.ContainsKey(item.BaseTextSnapshotLine);
        }

        public void CopyTo(TokenSnapshotLine[] array, int arrayIndex) {
            container.Values.CopyTo(array, arrayIndex);
        }

        bool ICollection<TokenSnapshotLine>.Remove(TokenSnapshotLine item) {
            return container.Remove(item.BaseTextSnapshotLine);
        }

        public void Clear() {
            container.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return container.Values.GetEnumerator();
        }
        public IEnumerator<TokenSnapshotLine> GetEnumerator() {
            return container.Values.GetEnumerator();
        }
    }
}
