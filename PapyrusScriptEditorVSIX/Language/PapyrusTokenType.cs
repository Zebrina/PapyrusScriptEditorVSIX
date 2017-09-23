using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public enum PapyrusTokenType {
        Null,
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
    }
}
