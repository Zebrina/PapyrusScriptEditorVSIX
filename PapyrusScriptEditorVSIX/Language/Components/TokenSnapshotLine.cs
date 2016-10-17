using Microsoft.VisualStudio.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Components {
    public interface IReadOnlyTokenSnapshotLine : IReadOnlyCollection<TokenInfo> {
        IEnumerable<TokenInfo> ParseableTokens { get; }
        bool IsEmpty { get; }
    }

    [DebuggerStepThrough]
    public sealed class TokenSnapshotLine : IReadOnlyTokenSnapshotLine, IReadOnlyCollection<TokenInfo>, ICollection<TokenInfo>, IEnumerable<TokenInfo>, IEnumerable {
        /*
        private class KeyComparer : IComparer<SnapshotSpan> {
            int IComparer<SnapshotSpan>.Compare(SnapshotSpan x, SnapshotSpan y) {
                return x.Start.CompareTo(y.Start);
            }
        }
        */
        
        private SortedList<SnapshotPoint, TokenInfo> container;

        public ITextSnapshotLine BaseTextSnapshotLine { get; private set; }

        public TokenSnapshotLine(ITextSnapshotLine baseTextSnapshotLine, IEnumerable<TokenInfo> collection) {
            this.BaseTextSnapshotLine = baseTextSnapshotLine;
            this.container = new SortedList<SnapshotPoint, TokenInfo>(collection.ToDictionary(t => t.Span.Start));
        }
        /*
        public TokenSnapshotLine() {
            this.BaseTextSnapshotLine = null;
            container = new SortedList<SnapshotPoint, TokenInfo>();
        }
        */

        public TokenInfo this[int position] {
            get { return position < container.Count ? container.Values[position] : null; }
        }
        public TokenInfo this[SnapshotPoint position] {
            get {
                TokenInfo value;
                if (container.TryGetValue(position, out value)) {
                    return value;
                }
                return null;
            }
        }
        public int Count {
            get { return container.Count; }
        }
        public bool IsEmpty {
            get { return container.Count == 0; }
        }
        bool ICollection<TokenInfo>.IsReadOnly {
            get { return container.Values.IsReadOnly; }
        }

        public IEnumerable<TokenInfo> ParseableTokens {
            get { return container.Values.Where(t => t.Type.IgnoredBySyntax == false); }
        }

        public void Add(TokenInfo item) {
            container.Add(item.Span.Start, item);
        }
        public void AddRange(IEnumerable<TokenInfo> collection) {
            foreach (var entry in collection) {
                container.Add(entry.Span.Start, entry);
            }
        }

        public bool Contains(TokenInfo item) {
            return container.ContainsKey(item.Span.Start);
        }

        public bool FindInSpan(SnapshotPoint point, out TokenInfo tokenInfoOut) {
            foreach (var tokenInfo in container.Values) {
                if (tokenInfo.Span.Contains(point)) {
                    tokenInfoOut = tokenInfo;
                    return true;
                }
            }

            tokenInfoOut = null;
            return false;
        }

        void ICollection<TokenInfo>.CopyTo(TokenInfo[] array, int arrayIndex) {
            container.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TokenInfo item) {
            return container.Remove(item.Span.Start);
        }

        public void Clear() {
            container.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return container.Values.GetEnumerator();
        }
        public IEnumerator<TokenInfo> GetEnumerator() {
            return container.Values.GetEnumerator();
        }
    }
}
