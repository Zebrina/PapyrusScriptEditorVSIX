using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Papyrus.Features;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Papyrus.Language.Components {
    /// <summary>
    /// Base implementation for ITokenType implementers.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class TokenType {
        #region Invalid token type definition

        public static readonly TokenType Invalid = new InvalidTokenType();

        private sealed class InvalidTokenType : TokenType {
            public override string Text {
                get { return String.Empty; }
            }
            public override TokenTypeID TypeID {
                get { return TokenTypeID.OtherOrInvalid; }
            }
            public override TokenColorID Color {
                get { return TokenColorID.Text; }
            }
            public override int Size {
                get { return 0; }
            }

            public override bool IgnoredInLine {
                get { return true; }
            }
        }

        #endregion

        public abstract string Text { get; }
        public abstract TokenTypeID TypeID { get; }
        public abstract TokenColorID Color { get; }
        public virtual int Size {
            get { return 1; }
        }

        public virtual IndentBehavior IndentBehavior {
            get { return IndentBehavior.DoNothing; }
        }

        public virtual bool IsOpeningBracer() {
            return false;
        }
        public virtual bool IsClosingBracer(TokenType openingBracer) {
            return false;
        }

        public virtual bool IsOutlineableStart(IReadOnlyParsedLine line) {
            return false;
        }
        public virtual bool IsOutlineableEnd(TokenType startToken) {
            return false;
        }

        public virtual bool CompileTimeConstant {
            get { return false; }
        }
        public virtual bool VariableType {
            get { return false; }
        }
        public virtual bool ExtendsLine {
            get { return false; }
        }
        public virtual bool IgnoredInLine {
            get { return false; }
        }

        public virtual IReadOnlyCollection<IScriptMember> GetMemberList() {
            return new IScriptMember[0];
        }

        public void CopyToTokenInfo(TokenInfo info, int offset) {
            info.StartIndex = offset;
            info.EndIndex = offset + Text.Length - 1;
            info.Type = (Microsoft.VisualStudio.Package.TokenType)TypeID;
            info.Color = (TokenColor)Color;
        }

        public override int GetHashCode() {
            return Text.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is TokenType && String.Equals(this.Text, ((TokenType)obj).Text, StringComparison.OrdinalIgnoreCase);
        }

        public int CompareTo(object obj) {
            if (obj is TokenType) {
                return CompareTo(obj as TokenType);
            }
            return String.Compare(this.Text, obj.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        public int CompareTo(TokenType other) {
            return String.Compare(this.Text, other.Text, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() {
            return Text;
        }

        public static bool IsNullOrInvalid(TokenType token) {
            return token == null || token.TypeID == TokenTypeID.OtherOrInvalid;
        }

        public static bool Equals(TokenType x, TokenType y) {
            if (x == null || y == null) {
                return ReferenceEquals(x, y);
            }
            return x.Equals(y);
        }
    }
}
