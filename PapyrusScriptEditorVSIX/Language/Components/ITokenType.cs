using Microsoft.VisualStudio.Package;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
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
        OtherOrInvalid,
        NumTypes,
    }

    public enum IndentBehavior {
        DoNothing,
        IncreaseAtNewLine,
        Decrease,
        DecreaseAndIncreaseAtNewLine,
    }

    /*
    public interface ITokenType : IComparable, IComparable<ITokenType> {
        string Text { get; }
        TokenTypeID TypeID { get; }
        TokenColor Color { get; }
        IndentBehavior IndentBehavior { get; }
        int Size { get; }
        bool IsOpeningBracer();
        bool IsClosingBracer(ITokenType openingBracer);
        bool IsOutlineableStart(IReadOnlyParsedLine line);
        bool IsOutlineableEnd(ITokenType startToken);
        bool CompileTimeConstant { get; }
        bool VariableType { get; }
        bool ExtendsLine { get; }
        bool IgnoredInLine { get; }
        IReadOnlyCollection<IScriptMember> GetMemberList();
        void CopyToTokenInfo(TokenInfo info, int offset);
    }
    */
}
