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

        public virtual bool IsOpeningBracer() {
            return false;
        }
        public virtual bool IsClosingBracer(Token openingBracer) {
            return false;
        }

        public virtual bool IsOutlineableStart(IReadOnlyTokenSnapshotLine line) {
            return false;
        }
        public virtual bool IsOutlineableEnd(Token startToken) {
            return false;
        }

        public virtual bool CompileTimeConstant {
            get { return false; }
        }
        public virtual bool IsVariableType {
            get { return false; }
        }
        public virtual bool ExtendsLine {
            get { return false; }
        }
        public virtual bool IgnoredInSyntax {
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
    }
}
