using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Dissecting {
    /// <summary>
    /// Represents a line that has been dissected by an ILineDissector implementation and contains key-value pairs according to the syntax it enforces.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class DissectedLine {
        private Dictionary<string, TokenType> valueTable;
        private Dictionary<string, List<TokenType>> arrayTable;

        public DissectedLine() {
            valueTable = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase);
            arrayTable = new Dictionary<string, List<TokenType>>(StringComparer.OrdinalIgnoreCase);
        }

        public bool AddEntry(string key, TokenType value) {
            if (!String.IsNullOrWhiteSpace(key) && !valueTable.ContainsKey(key)) {
                valueTable.Add(key, value);
                return true;
            }
            return false;
        }
        public bool AddArrayEntry(string key, TokenType value) {
            if (!String.IsNullOrWhiteSpace(key)) {
                List<TokenType> array = GetArray(key);
                if (!array.Contains(value)) {
                    array.Add(value);
                    return true;
                }
            }
            return false;
        }

        public bool ContainsEntryWithKey(string key) {
            return valueTable.ContainsKey(key);
        }
        public bool ContainsEntry(string key, TokenType value) {
            return valueTable.Contains(new KeyValuePair<string, TokenType>(key, value));
        }
        public bool ContainsArrayWithKey(string key) {
            return GetArray(key).Count > 0;
        }
        public bool ContainsArrayEntry(string key, TokenType value) {
            return GetArray(key).Contains(value);
        }

        public TokenType GetEntry(string key) {
            TokenType token;
            valueTable.TryGetValue(key, out token);
            return token;
        }
        public IEnumerable<TokenType> GetArrayEntries(string key) {
            return GetArray(key);
        }

        /// <summary>
        /// Returns an array associated with a specific key. If no array associated with the key exists, a new array will be created and stored in the array table.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<TokenType> GetArray(string key) {
            List<TokenType> array;
            if (arrayTable.TryGetValue(key, out array)) {
                return array;
            }
            array = new List<TokenType>();
            arrayTable.Add(key, array);
            return array;
        }
    }
}
