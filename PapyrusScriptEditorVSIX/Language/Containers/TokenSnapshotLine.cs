using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class TokenSnapshotLine : IList<Token>, IReadOnlyList<Token>, ICollection<Token>, IReadOnlyCollection<Token>, IEnumerable<Token>, IEnumerable {
        private List<Token> container;

        public TokenSnapshot() {
            container = new List<Token>();
        }

        public Token operator this[int position] {
            get { return container[position]; }
        }
        Token operator IList<Token>.this[int position] {
            get { return container[position]; }
            set { container[position] = value; }
        }
        public int Count {
            get { return container.Count; }
        }
        bool ICollection<Token>.IsReadOnly {
            get { return ((ICollection<Token>)container).IsReadOnly; }
        }

        void ICollection<Token>.Add(Token item) {
            container.Add(item);
        }
        void IList<Token>.Insert(int index, Token item) {
            container.Insert(index, item);
        }

        public bool Contains(Token item) {
            return container.Contains(item);
        }

        public int IndexOf(Token item) {
            return container.IndexOf(item);
        }

        public void CopyTo(Token[] array, int arrayIndex) {
            container.CopyTo(array, arrayIndex);
        }

        bool ICollection<Token>.Remove(Token item) {
            return container.Remove(item);
        }
        void IList<Token>.RemoveAt(int index) {
            container.RemoveAt(index);
        }

        void ICollection<Token>.Clear() {
            container.Clear();
        }

        IEnumerable IEnumerable.GetEnumerator() {
            return container.GetEnumerator();
        }
        public IEnumerator<Token> GetEnumerator() {
            return container.GetEnumerator();
        }
    }
}
