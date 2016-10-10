using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Language.Components {
    public struct ScriptMemberAttributes : ISyntaxParsable, IEnumerable<Keyword> {
        private HashSet<Keyword> attributes;

        public ScriptMemberAttributes(params Keyword[] keywordAttributes) {
            attributes = new HashSet<Keyword>(keywordAttributes);
        }

        public bool Add(Keyword attributeKeyword) {
            if (attributes == null) {
                attributes = new HashSet<Keyword>();
            }
            return attributes.Add(attributeKeyword);
        }
        public bool AddRange(IEnumerable<Keyword> collection) {
            bool result = false;
            foreach (var item in collection) {
                result = Add(item) || result;
            }
            return result;
        }
        public bool Contains(Keyword attributeKeyword) {
            return attributes != null && attributes.Contains(attributeKeyword);
        }
        public bool Remove(Keyword attributeKeyword) {
            return attributes != null && attributes.Remove(attributeKeyword);
        }
        private void Clear() {
            if (attributes != null) {
                attributes.Clear();
                attributes = null;
            }
        }

        public bool this[Keyword attributeKeyword] {
            get { return Contains(attributeKeyword); }
        }

        public int Length {
            get { return attributes == null ? 0 : attributes.Count; }
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            Clear();
            foreach (Token token in tokens.Skip(offset)) {
                if (!(token is Keyword && ((Keyword)token).Attributes.HasFlag(KeywordAttribute.Attribute))) {
                    break;
                }
                Add((Keyword)token);
            }
            return true;
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            foreach (var keywordAttribute in attributes) {
                b.AppendWhiteSpace();
                b.Append(keywordAttribute);
            }

            return b.ToString();
        }

        public IEnumerator<Keyword> GetEnumerator() {
            return attributes.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return attributes.GetEnumerator();
        }
    }
}
