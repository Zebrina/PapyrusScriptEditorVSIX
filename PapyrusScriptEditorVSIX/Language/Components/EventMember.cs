using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    [DebuggerStepThrough]
    public sealed class EventMember : IScriptMember, ICollection<Parameter>, IEnumerable<Parameter> {
        private List<Parameter> parameters;

        private string name;
        private string definition;

        public EventMember(string name, IEnumerable<Parameter> parameters) {
            this.name = name;
            this.parameters = new List<Parameter>(parameters);
        }
        public EventMember(string name) :
            this(name, new Parameter[0]) {
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            b.Append(Keyword.Event);
            b.Append(' ');
            b.Append(name);

            b.Append(Delimiter.LeftRoundBracket);

            if (parameters.Count > 0) {
                b.Append(parameters[0]);
                for (int i = 1; i < parameters.Count; ++i) {
                    b.Append(", ");
                    b.Append(parameters[i]);
                }
            }

            b.Append(Delimiter.RightRoundBracket);

            return b.ToString();
        }
        public string GetDefinition() {
            return definition;
        }
        bool IScriptMember.ChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return parameters; }
        }
        bool IScriptMember.Hidden {
            get { return false; }
        }

        public override string ToString() {
            return GetDeclaration();
        }

        int ICollection<Parameter>.Count {
            get { return parameters.Count; }
        }
        bool ICollection<Parameter>.IsReadOnly {
            get { return ((ICollection<Parameter>)parameters).IsReadOnly; }
        }

        public int CompareTo(IScriptMember obj) {
            return String.Compare(this.name, obj.Name, StringComparison.OrdinalIgnoreCase);
        }

        void ICollection<Parameter>.Add(Parameter item) {
            parameters.Add(item);
        }

        bool ICollection<Parameter>.Contains(Parameter item) {
            return parameters.Contains(item);
        }

        void ICollection<Parameter>.CopyTo(Parameter[] array, int arrayIndex) {
            parameters.CopyTo(array, arrayIndex);
        }

        bool ICollection<Parameter>.Remove(Parameter item) {
            return parameters.Remove(item);
        }

        void ICollection<Parameter>.Clear() {
            parameters.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return parameters.GetEnumerator();
        }
        IEnumerator<Parameter> IEnumerable<Parameter>.GetEnumerator() {
            return parameters.GetEnumerator();
        }
    }
}
