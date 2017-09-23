using Papyrus.Language.Components;
using System.Collections.Generic;
using System.Diagnostics;

namespace Papyrus.Language {
    public enum TokenTypeID {
        WhiteSpace,
        Text,
        Comment,
        LineComment,
        CreationKitInfo,
        String,
        NumericLiteral,
        Braces,
        Operator,
        Delimiter,
        Keyword,
        ScriptObject,
        Identifier,
        NumTypes,
    }

    public enum IndentBehavior {
        DoNothing,
        IncreaseAtNewLine,
        Decrease,
        DecreaseAndIncreaseAtNewLine,
    }

    [DebuggerStepThrough]
    public abstract class Token {
        public abstract string Text { get; }
        public abstract TokenTypeID TypeID { get; }

        public virtual IndentBehavior IndentBehavior {
            get { return IndentBehavior.DoNothing; }
        }

        public virtual bool IsOpeningBracer {
            get { return false; }
        }
        public virtual bool IsClosingBracer {
            get { return false; }
        }
        public virtual bool MatchesWithBracer(Token otherBracer) {
            return false;
        }

        public virtual bool IsCompileTimeConstant {
            get { return false; }
        }
        public virtual bool IsVariableType {
            get { return false; }
        }
        public virtual bool ExtendsLine {
            get { return false; }
        }
        public virtual bool IgnoredBySyntax {
            get { return false; }
        }

        public virtual IReadOnlyCollection<IScriptMember> GetMemberList() {
            return new IScriptMember[0];
        }

        public override string ToString() {
            return Text;
        }

        public static explicit operator string(Token token) {
            return token.ToString();
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public static bool operator ==(Token x, Token y) {
            return Equals(x, y);
        }
        public static bool operator !=(Token x, Token y) {
            return !Equals(x, y);
        }

        /*
        public static Token Parse(string tokenStr) {
            return null;
        }
        public static bool TryParse(string tokenStr, Token token) {
            return false;
        }
        */
    }
}
