using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Papyrus.Language.Components {
    //[DebuggerStepThrough]
    public sealed class FunctionMember : IScriptMember, IComparable<IScriptMember>, ICloneable {
        private VariableType returnType;
        private string name;
        private readonly ParameterList parameters;
        private ScriptMemberAttributes attributes;
        private string definition;

        /*
        public FunctionMember(VariableType returnType, string name, IEnumerable<Parameter> parameters, IEnumerable<Keyword> attributes) {
            this.returnType = returnType;
            this.name = name;
            this.parameters = new ParameterList(parameters);
            this.attributes = new ScriptMemberAttributes();
            this.attributes.AddRange(attributes);
            this.definition = String.Empty;
        }
        public FunctionMember(VariableType returnType, string name, IEnumerable<Parameter> parameters) :
            this(returnType, name, parameters, new Keyword[0]) {
        }
        public FunctionMember(VariableType returnType, string name, IEnumerable<Keyword> attributes) :
            this(returnType, name, new Parameter[0], attributes) {
        }
        public FunctionMember(string name, IEnumerable<Parameter> parameters, IEnumerable<Keyword> attributes) :
            this(default(VariableType), name, parameters, attributes) {
        }
        public FunctionMember(string name, IEnumerable<Keyword> attributes) :
            this(default(VariableType), name, new Parameter[0], attributes) {
        }
        */
        private FunctionMember(FunctionMember other) {
            this.returnType = other.returnType;
            this.name = other.name;
            this.parameters = (ParameterList)other.parameters.Clone();
            this.attributes = other.attributes;
            this.definition = other.definition;
        }
        private FunctionMember() {
            returnType = default(VariableType);
            name = String.Empty;
            parameters = new ParameterList();
            attributes = default(ScriptMemberAttributes);
            definition = String.Empty;
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            b.Append(returnType);
            b.Append(Keyword.Function);
            b.AppendWhiteSpace();
            b.Append(name);
            b.AppendFormat("({0})", parameters);
            b.Append(attributes);

            return b.ToString();
        }
        public string GetDefinition() {
            return definition;
        }
        bool IScriptMember.IsChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return new Parameter[0]; }
        }
        bool IScriptMember.Hidden {
            get { return false; }
        }

        public int Length {
            get {
                int length = returnType.Length + 4 + parameters.Length; // function + <name> + ( + parameters + );
                if (attributes.Contains(Keyword.Global)) length += 1;
                if (attributes.Contains(Keyword.Native)) length += 1;

                return length;
            }
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            int remainingCount = tokens.Count - offset;
            if (remainingCount >= 4) {
                if (returnType.TryParse(tokens, offset)) {
                    offset += returnType.Length;
                }

                remainingCount = tokens.Count - offset;
                if (remainingCount >= 4 && tokens[offset] == Keyword.Function && tokens[offset + 1].TypeID == TokenTypeID.Identifier && tokens[offset + 2] == Delimiter.LeftRoundBracket) {
                    name = tokens[offset + 1].Text;
                    offset += 3;

                    parameters.TryParse(tokens, offset);
                    offset += parameters.Length;

                    remainingCount = tokens.Count - offset;
                    if (remainingCount >= 1 && tokens[offset] == Delimiter.RightRoundBracket) {
                        offset += 1;
                        attributes.TryParse(tokens, offset);

                        return true;
                    }
                }
            }

            return false;
        }

        private static FunctionMember prototype = new FunctionMember();
        public static bool TryParse(IReadOnlyList<Token> tokens, int offset, out IScriptMember result) {
            if (prototype.TryParse(tokens, offset)) {
                result = (FunctionMember)prototype.MemberwiseClone();
                return true;
            }
            result = null;
            return false;
        }

        public int CompareTo(IScriptMember obj) {
            return String.Compare(this.name, obj.Name, StringComparison.OrdinalIgnoreCase);
        }

        public object Clone() {
            return new FunctionMember(this);
        }

        public override string ToString() {
            return GetDeclaration();
        }
    }
}
