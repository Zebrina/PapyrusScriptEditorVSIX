using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Papyrus.Language.Components {
    public class ParameterList : ISyntaxParsable, IList<Parameter>, IReadOnlyList<Parameter>, ICollection<Parameter>, IReadOnlyCollection<Parameter>, IEnumerable<Parameter>, IEnumerable, ICloneable {
        private readonly List<Parameter> internalList;

        public ParameterList(IEnumerable<Parameter> collection) {
            internalList = new List<Parameter>(collection);
        }
        public ParameterList() {
            internalList = new List<Parameter>();
        }

        public Parameter this[int index] {
            get { return internalList[index]; }
            set { internalList[index] = value; }
        }

        public int Count {
            get { return internalList.Count; }
        }
        public bool IsReadOnly {
            get { return ((IList<Parameter>)this.internalList).IsReadOnly; }
        }

        public int Length {
            get { return internalList.Sum(p => p.Length) + (internalList.Count - 1); }
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            internalList.Clear();
            Parameter p = new Parameter();
            while (p.TryParse(tokens, offset)) {
                offset += p.Length;
                internalList.Add(p);
                if (tokens[offset] != Delimiter.Comma) {
                    break;
                }
                offset += 1;
            }
            return true;
        }

        public void Add(Parameter item) {
            internalList.Add(item);
        }
        public void AddRange(IEnumerable<Parameter> collection) {
            internalList.AddRange(collection);
        }
        public void Insert(int index, Parameter item) {
            internalList.Insert(index, item);
        }

        public bool Contains(Parameter item) {
            return internalList.Contains(item);
        }
        public int IndexOf(Parameter item) {
            return internalList.IndexOf(item);
        }

        public void CopyTo(Parameter[] array, int arrayIndex) {
            internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Parameter item) {
            return internalList.Remove(item);
        }

        public void RemoveAt(int index) {
            internalList.RemoveAt(index);
        }

        public void Clear() {
            internalList.Clear();
        }

        public IEnumerator<Parameter> GetEnumerator() {
            return internalList.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return internalList.GetEnumerator();
        }

        public object Clone() {
            return new ParameterList(this);
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();
            bool first = true;
            internalList.ForEach(p => {
                if (first) {
                    first = false;
                }
                else {
                    b.Append(", ");
                }

                b.Append(p);
            });
            return b.ToString();
        }
    }
}
