using Microsoft.VisualStudio.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Components {
    public interface IReadOnlyTokenSnapshotLine : IReadOnlyCollection<PapyrusTokenInfo> {
        IEnumerable<PapyrusTokenInfo> ParseableTokens { get; }
        bool IsEmpty { get; }
    }

    [DebuggerStepThrough]
    public sealed class TokenSnapshotLine : IReadOnlyTokenSnapshotLine, IReadOnlyCollection<PapyrusTokenInfo>, ICollection<PapyrusTokenInfo>, IEnumerable<PapyrusTokenInfo>, IEnumerable {
        /*
        private class KeyComparer : IComparer<SnapshotSpan> {
            int IComparer<SnapshotSpan>.Compare(SnapshotSpan x, SnapshotSpan y) {
                return x.Start.CompareTo(y.Start);
            }
        }
        */
        
        private SortedList<SnapshotPoint, PapyrusTokenInfo> container;
        private List<ITextSnapshotLine> baseTextSnapshotLines;

        //[Obsolete]
        //public SnapshotSpan BaseTextSnapshotLine { get; private set; }

        public TokenSnapshotLine(IEnumerable<PapyrusTokenInfo> collection) {
            //this.BaseTextSnapshotLine = baseTextSnapshotLine.Extent;
            this.container = new SortedList<SnapshotPoint, PapyrusTokenInfo>(collection.ToDictionary(t => t.Span.Start));
            this.baseTextSnapshotLines = new List<ITextSnapshotLine>();
        }
        /*
        public TokenSnapshotLine() {
            this.BaseTextSnapshotLine = null;
            container = new SortedList<SnapshotPoint, TokenInfo>();
        }
        */

        public PapyrusTokenInfo this[int position] {
            get { return position < container.Count ? container.Values[position] : null; }
        }
        public PapyrusTokenInfo this[SnapshotPoint position] {
            get {
                PapyrusTokenInfo value;
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
        bool ICollection<PapyrusTokenInfo>.IsReadOnly {
            get { return container.Values.IsReadOnly; }
        }

        public IEnumerable<PapyrusTokenInfo> ParseableTokens {
            get { return container.Values.Where(t => t.Type.IgnoredBySyntax == false); }
        }

        public void Add(PapyrusTokenInfo item) {
            container.Add(item.Span.Start, item);
        }
        public void AddRange(IEnumerable<PapyrusTokenInfo> collection) {
            foreach (var entry in collection) {
                container.Add(entry.Span.Start, entry);
            }
        }

        public bool Contains(PapyrusTokenInfo item) {
            return container.ContainsKey(item.Span.Start);
        }

        public bool FindInSpan(SnapshotPoint point, out PapyrusTokenInfo tokenInfoOut) {
            foreach (var tokenInfo in container.Values) {
                if (tokenInfo.Span.Contains(point)) {
                    tokenInfoOut = tokenInfo;
                    return true;
                }
            }

            tokenInfoOut = null;
            return false;
        }

        void ICollection<PapyrusTokenInfo>.CopyTo(PapyrusTokenInfo[] array, int arrayIndex) {
            container.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(PapyrusTokenInfo item) {
            return container.Remove(item.Span.Start);
        }

        public void Clear() {
            container.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return container.Values.GetEnumerator();
        }
        public IEnumerator<PapyrusTokenInfo> GetEnumerator() {
            return container.Values.GetEnumerator();
        }
    }
}
