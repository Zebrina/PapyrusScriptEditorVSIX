using Microsoft.VisualStudio.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Parsing {
    public interface IReadOnlyTokenSnapshotLine : IReadOnlyCollection<PapyrusTokenInfo>, IEnumerable<PapyrusTokenInfo>, IEnumerable {
        IEnumerable<PapyrusTokenInfo> GetParseableTokensWithInfo();
        IEnumerable<IPapyrusToken> GetParseableTokens();
    }

    public interface IAppendableTokenSnapshotLine : IReadOnlyTokenSnapshotLine, IReadOnlyCollection<PapyrusTokenInfo>, IEnumerable<PapyrusTokenInfo>, IEnumerable {
        void Add(PapyrusTokenInfo item);
    }

    [DebuggerStepThrough]
    public sealed class TokenSnapshotLine : IReadOnlyTokenSnapshotLine, IReadOnlyCollection<PapyrusTokenInfo>, IEnumerable<PapyrusTokenInfo>, IEnumerable, IAppendableTokenSnapshotLine, ICollection<PapyrusTokenInfo> {
        /*
        private class KeyComparer : IComparer<SnapshotSpan> {
            int IComparer<SnapshotSpan>.Compare(SnapshotSpan x, SnapshotSpan y) {
                return x.Start.CompareTo(y.Start);
            }
        }
        */
        
        private SortedList<SnapshotPoint, PapyrusTokenInfo> tokens;
        private List<ITextSnapshotLine> baseTextSnapshotLines;

        //[Obsolete]
        //public SnapshotSpan BaseTextSnapshotLine { get; private set; }

        public TokenSnapshotLine(IEnumerable<PapyrusTokenInfo> collection) {
            //this.BaseTextSnapshotLine = baseTextSnapshotLine.Extent;
            this.tokens = new SortedList<SnapshotPoint, PapyrusTokenInfo>(collection.ToDictionary(t => t.Span.Start));
            this.baseTextSnapshotLines = new List<ITextSnapshotLine>();
        }
        /*
        public TokenSnapshotLine() {
            this.BaseTextSnapshotLine = null;
            container = new SortedList<SnapshotPoint, TokenInfo>();
        }
        */

        public PapyrusTokenInfo this[int position] {
            get { return position < tokens.Count ? tokens.Values[position] : null; }
        }
        public PapyrusTokenInfo this[SnapshotPoint position] {
            get {
                PapyrusTokenInfo value;
                if (tokens.TryGetValue(position, out value)) {
                    return value;
                }
                return null;
            }
        }
        public int Count {
            get { return tokens.Count; }
        }
        bool ICollection<PapyrusTokenInfo>.IsReadOnly {
            get { return tokens.Values.IsReadOnly; }
        }

        public IEnumerable<PapyrusTokenInfo> GetParseableTokensWithInfo() {
            return tokens.Values.Where(t => t.Type.IsIgnoredByParser);
        }
        public IEnumerable<IPapyrusToken> GetParseableTokens() {
            return GetParseableTokensWithInfo().Select(t => t.Type);
        }

        public void Add(PapyrusTokenInfo item) {
            tokens.Add(item.Span.Start, item);
        }
        public void AddRange(IEnumerable<PapyrusTokenInfo> collection) {
            foreach (var entry in collection) {
                tokens.Add(entry.Span.Start, entry);
            }
        }

        public bool Contains(PapyrusTokenInfo item) {
            return tokens.ContainsKey(item.Span.Start);
        }

        public bool FindInSpan(SnapshotPoint point, out PapyrusTokenInfo tokenInfoOut) {
            foreach (var tokenInfo in tokens.Values) {
                if (tokenInfo.Span.Contains(point)) {
                    tokenInfoOut = tokenInfo;
                    return true;
                }
            }

            tokenInfoOut = null;
            return false;
        }

        public void CopyTo(PapyrusTokenInfo[] array, int arrayIndex) {
            tokens.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(PapyrusTokenInfo item) {
            return tokens.Remove(item.Span.Start);
        }

        public void Clear() {
            tokens.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return tokens.Values.GetEnumerator();
        }
        public IEnumerator<PapyrusTokenInfo> GetEnumerator() {
            return tokens.Values.GetEnumerator();
        }
    }
}
