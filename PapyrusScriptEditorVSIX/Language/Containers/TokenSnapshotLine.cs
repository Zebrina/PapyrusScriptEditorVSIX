using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public interface IReadOnlyTokenSnapshotLine : IReadOnlyList<Token>, IReadOnlyCollection<Token> {
        bool IsEmpty { get; }
    }

    [DebuggerStepThrough]
    public class TokenSnapshotLine : IReadOnlyTokenSnapshotLine, IList<Token>, IReadOnlyList<Token>, ICollection<Token>, IReadOnlyCollection<Token>, IEnumerable<Token>, IEnumerable {
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
        public bool IsEmpty {
            get { return container.Count > 0; }
        }
        bool ICollection<Token>.IsReadOnly {
            get { return ((ICollection<Token>)container).IsReadOnly; }
        }

        public bool AddToken(Token token) {
            if (token != null) {
                container.Add(token);
                return true;
            }
            return false;
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

        public void Clear() {
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
