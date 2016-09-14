using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class TokenSnapshot : IList<TokenSnapshotLine>, IReadOnlyList<TokenSnapshotLine>, ICollection<TokenSnapshotLine>, IReadOnlyCollection<TokenSnapshotLine>, IEnumerable<TokenSnapshotLine>, IEnumerable {
        private List<TokenSnapshotLine> container;

        public TokenSnapshot() {
            container = new List<TokenSnapshotLine>();
        }

        public TokenSnapshotLine operator this[int position] {
            get { return container[position]; }
        }
        TokenSnapshotLine operator IList<TokenSnapshotLine>.this[int position] {
            get { return container[position]; }
            set { container[position] = value; }
        }
        public int Count {
            get { return container.Count; }
        }
        bool ICollection<TokenSnapshotLine>.IsReadOnly {
            get { return ((ICollection<TokenSnapshotLine>)container).IsReadOnly; }
        }

        void ICollection<TokenSnapshotLine>.Add(TokenSnapshotLine item) {
            container.Add(item);
        }
        void IList<TokenSnapshotLine>.Insert(int index, TokenSnapshotLine item) {
            container.Insert(index, item);
        }

        public bool Contains(TokenSnapshotLine item) {
            return container.Contains(item);
        }

        public int IndexOf(TokenSnapshotLine item) {
            return container.IndexOf(item);
        }

        public void CopyTo(TokenSnapshotLine[] array, int arrayIndex) {
            container.CopyTo(array, arrayIndex);
        }

        bool ICollection<TokenSnapshotLine>.Remove(TokenSnapshotLine item) {
            return container.Remove(item);
        }
        void IList<TokenSnapshotLine>.RemoveAt(int index) {
            container.RemoveAt(index);
        }

        void ICollection<TokenSnapshotLine>.Clear() {
            container.Clear();
        }

        IEnumerable IEnumerable.GetEnumerator() {
            return container.GetEnumerator();
        }
        public IEnumerator<TokenSnapshotLine> GetEnumerator() {
            return container.GetEnumerator();
        }
    }
}
