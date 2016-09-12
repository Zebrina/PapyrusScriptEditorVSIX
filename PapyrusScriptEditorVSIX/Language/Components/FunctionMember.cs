using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    [DebuggerStepThrough]
    public sealed class FunctionMember : IScriptMember, ICollection<Parameter>, IEnumerable<Parameter> {
        private TokenType returnType;
        private string name;
        private List<Parameter> parameters;
        private bool global;
        private bool native;
        private string definition;

        public FunctionMember(TokenType returnType, string name, IEnumerable<Parameter> parameters, bool global = false, bool native = false) {
            this.returnType = returnType;
            this.name = name;
            this.global = global;
            this.native = native;
            this.parameters = new List<Parameter>(parameters);
        }
        public FunctionMember(TokenType returnType, string name, bool global = false, bool native = false) :
            this(returnType, name, new Parameter[0], global, native) {
        }
        public FunctionMember(string name, IEnumerable<Parameter> parameters, bool global = false, bool native = false) :
            this(null, name, parameters, global, native) {
        }
        public FunctionMember(string name, bool global = false, bool native = false) :
            this(null, name, new Parameter[0], global, native) {
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            if (returnType != null) {
                b.Append(returnType.Text);
                b.Append(' ');
            }

            b.Append(Keyword.Function);
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

            if (global) {
                b.Append(' ');
                b.Append(Keyword.Global);
            }

            if (native) {
                b.Append(' ');
                b.Append(Keyword.Native);
            }

            return b.ToString();
        }
        public string GetDefinition() {
            return definition;
        }
        bool IScriptMember.ChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return new Parameter[0]; }
        }
        bool IScriptMember.Hidden {
            get { return false; }
        }

        public override string ToString() {
            return GetDeclaration();
        }

        int IComparable<IScriptMember>.CompareTo(IScriptMember other) {
            return String.Compare(this.name, other.Name, StringComparison.OrdinalIgnoreCase);
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
