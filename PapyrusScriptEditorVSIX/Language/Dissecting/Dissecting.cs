#if false
using Papyrus.Language.Components;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Dissecting {
    public static class Dissecting {
        public static TokenType ExtractVariableType(ScriptParser parser, IReadOnlyParsedLine line, int position) {
            TokenType type = null;
            if (line[position].TypeID == TokenTypeID.Identifier) {
                type = parser.ParseScript(line[position].Text);
            }
            else if (line[position].VariableType) {
                type = line[position];
            }

            if (type != null) {
                if (TokenType.Equals(line[position + 1], Delimiter.LeftSquareBracket) && TokenType.Equals(line[position + 2], Delimiter.RightSquareBracket)) {
                    type = new ArrayType(type);
                }

                return type;
            }

            return TokenType.Invalid;
        }
    }
}

#endif