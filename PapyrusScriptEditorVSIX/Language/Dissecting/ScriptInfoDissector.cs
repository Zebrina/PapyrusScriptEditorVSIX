#if false
using Papyrus.Language.Components;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Dissecting {
    public class ScriptInfoDissector : ILineDissector {
        private readonly string key;

        [DebuggerStepThrough]
        public ScriptInfoDissector(string key) {
            Debug.Assert(key != null, "'key' can't be null.");

            this.key = key;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position == 0 && line.Count >= 2 && TokenType.Equals(line[0], Keyword.Scriptname) && line[1].TypeID == TokenTypeID.Identifier) {
                result.AddEntry(Keyword.Scriptname, line[1]);

                int offset = 2;

                if (line.Count >= 4 && TokenType.Equals(line[2], Keyword.Extends)) {
                    ScriptObject parent = null;
                    if (line[3].TypeID == TokenTypeID.Identifier) {
                        parent = parser.ParseScript(line[3].Text);
                    }
                    else if (line[3].TypeID == TokenTypeID.ScriptObject) {
                        parent = (ScriptObject)line[3];
                    }

                    if (parent == null) {
                        return -1;
                    }

                    result.AddEntry(Keyword.Extends, parent);

                    offset += 2;
                }

                AttributesDissector a = new AttributesDissector(key, Keyword.Conditional, Keyword.Hidden);
                offset += a.DissectLine(parser, line, position + offset, result);

                return offset;
            }

            return -1;
        }
    }
}

#endif