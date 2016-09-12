using Papyrus.Language.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Parsing {
    public interface IReadOnlyParsedLine : IReadOnlyList<Token> {
        //Token this[int index] { get; }
        bool Contains(Token item);
        //int IndexOf(IToken item);
    }

    /// <summary>
    /// Represents a source line that has been parsed and contains a variable number of tokens.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ParsedLine : IReadOnlyParsedLine, ICollection<Token>, IEnumerable<Token> {
        private List<Token> tokenList;

        private Token TryGetToken(int index) {
            if (index >= 0 && index < tokenList.Count) {
                return tokenList[index];
            }
            return Token.Null;
        }

        public Token /*IReadOnlyList<IToken>.*/this[int index] {
            get { return TryGetToken(index); }
        }
        /*
        public IToken this[int index] {
            get { return TryGetToken(index); }
            set {
                if (index >= 0 && index < tokenList.Count) {
                    tokenList[index] = value;
                }
            }
        }
        */
        public int Count {
            get { return tokenList.Count; }
        }
        bool ICollection<Token>.IsReadOnly {
            get { return ((IList<Token>)tokenList).IsReadOnly; }
        }

        public ParsedLine() {
            tokenList = new List<Token>();
        }
        /*
        public ParsedLine(params Token[] tokens) :
            this() {
            tokenList.AddRange(tokens);
        }
        public ParsedLine(IEnumerable<Token> tokens) :
            this() {
            tokenList.AddRange(tokens);
        }
        */

        public void Add(Token item) {
            tokenList.Add(item);
        }
        /*
        public void Insert(int index, IToken item) {
            tokenList.Insert(index, item);
        }
        */

        public bool Remove(Token item) {
            return tokenList.Remove(item);
        }
        /*
        public void RemoveAt(int index) {
            tokenList.RemoveAt(index);
        }
        */

        public void Clear() {
            tokenList.Clear();
        }

        public bool Contains(Token item) {
            return tokenList.Contains(item);
        }
        public int IndexOf(Token item) {
            return tokenList.IndexOf(item);
        }
        public int IndexOf(Token item, int offset) {
            for (; offset < tokenList.Count; ++offset) {
                if (tokenList[offset].Equals(item)) {
                    return offset;
                }
            }
            return -1;
        }

        public IEnumerator GetEnumerator() {
            return tokenList.GetEnumerator();
        }
        IEnumerator<Token> IEnumerable<Token>.GetEnumerator() {
            return tokenList.GetEnumerator();
        }

        public void CopyTo(Token[] array, int arrayIndex) {
            tokenList.CopyTo(array, arrayIndex);
        }
    }
}