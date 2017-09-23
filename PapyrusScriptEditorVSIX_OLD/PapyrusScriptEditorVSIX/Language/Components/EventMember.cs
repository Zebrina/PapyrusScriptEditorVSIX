using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Papyrus.Language.Components {
    //[DebuggerStepThrough]
    public sealed class EventMember : IScriptMember, IComparable<IScriptMember>, ICloneable {
        private string name;
        private ParameterList parameters;
        private string definition;

        /*
        public EventMember(string name, IEnumerable<Parameter> parameters) {
            this.name = name;
            this.parameters = new ParameterList(parameters);
            this.definition = String.Empty;
        }
        public EventMember(string name) :
            this(name, new Parameter[0]) {
        }
        */
        private EventMember(EventMember other) {
            this.name = other.name;
            this.parameters = (ParameterList)other.parameters.Clone();
            this.definition = other.definition;
        }
        private EventMember() {
            name = String.Empty;
            parameters = new ParameterList();
            definition = String.Empty;
        }

        public string Name {
            get { return name; }
        }
        public string GetDeclaration() {
            StringBuilder b = new StringBuilder();

            b.Append(Keyword.Event);
            b.AppendWhiteSpace();
            b.Append(name);

            b.Append(Delimiter.LeftRoundBracket);
            b.Append(parameters);
            b.Append(Delimiter.RightRoundBracket);

            return b.ToString();
        }
        public string GetDefinition() {
            return definition;
        }
        bool IScriptMember.IsChildAccessible {
            get { return true; }
        }
        IEnumerable<Parameter> IScriptMember.Parameters {
            get { return parameters; }
        }
        bool IScriptMember.Hidden {
            get { return false; }
        }

        public int Length {
            get { return 4 + parameters.Length; } // event + <name> + ( + parameters + )
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            int remainingCount = tokens.Count - offset;
            if (remainingCount >= 4 && tokens[offset] == Keyword.Function && tokens[offset + 1].TypeID == TokenTypeID.Identifier && tokens[offset + 2] == Delimiter.LeftRoundBracket) {
                name = tokens[offset + 1].Text;

                offset = 3;
                parameters.TryParse(tokens, offset + 3);
                offset += parameters.Length;

                remainingCount = tokens.Count - offset;
                return remainingCount >= 1 && tokens[offset] == Delimiter.RightRoundBracket;
            }

            return false;
        }

        private static EventMember prototype = new EventMember();
        public static bool TryParse(IReadOnlyList<Token> tokens, int offset, out IScriptMember result) {
            if (prototype.TryParse(tokens, offset)) {
                result = (EventMember)prototype.MemberwiseClone();
                return true;
            }
            result = null;
            return false;
        }

        int IComparable<IScriptMember>.CompareTo(IScriptMember obj) {
            return String.Compare(this.name, obj.Name, StringComparison.OrdinalIgnoreCase);
        }

        public object Clone() {
            return new EventMember(this);
        }

        public override string ToString() {
            return GetDeclaration();
        }
    }
}
