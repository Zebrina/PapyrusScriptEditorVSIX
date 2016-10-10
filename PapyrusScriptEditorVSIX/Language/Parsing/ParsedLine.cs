#if false
using Papyrus.Language.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Parsing {
    public interface IReadOnlyParsedLine : IReadOnlyList<TokenInfo> {
        //Token this[int index] { get; }
        bool Contains(TokenInfo item);
        //int IndexOf(IToken item);
    }

    /// <summary>
    /// Represents a source line that has been parsed and contains a variable number of tokens.
    /// </summary>
    //[DebuggerStepThrough]
    public sealed class ParsedLine : IReadOnlyParsedLine, ICollection<TokenInfo>, IEnumerable<TokenInfo> {
        private List<TokenInfo> tokenList;

        private TokenInfo TryGetToken(int index) {
            if (index >= 0 && index < tokenList.Count) {
                return tokenList[index];
            }
            return null;
        }

        public TokenInfo /*IReadOnlyList<IToken>.*/this[int index] {
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
        bool ICollection<TokenInfo>.IsReadOnly {
            get { return ((IList<TokenInfo>)tokenList).IsReadOnly; }
        }

        public ParsedLine() {
            tokenList = new List<TokenInfo>();
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

        public void Add(TokenInfo item) {
            tokenList.Add(item);
        }
        /*
        public void Insert(int index, IToken item) {
            tokenList.Insert(index, item);
        }
        */

        public bool Remove(TokenInfo item) {
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

        public bool Contains(TokenInfo item) {
            return tokenList.Contains(item);
        }
        public int IndexOf(TokenInfo item) {
            return tokenList.IndexOf(item);
        }
        public int IndexOf(TokenInfo item, int offset) {
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
        IEnumerator<TokenInfo> IEnumerable<TokenInfo>.GetEnumerator() {
            return tokenList.GetEnumerator();
        }

        public void CopyTo(TokenInfo[] array, int arrayIndex) {
            tokenList.CopyTo(array, arrayIndex);
        }
    }
} 
#endif