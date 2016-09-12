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
    public class EventDissector : ILineDissector {
        private readonly string key;

        [DebuggerStepThrough]
        public EventDissector(string key) {
            this.key = key;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position == 0 && line.Count >= 4 && TokenType.Equals(line[0], Keyword.Event) && line[1].TypeID == TokenTypeID.Identifier) {
                result.AddEntry(Keyword.Event, line[1]);

                ParameterListDissector p = new ParameterListDissector(key);
                int offset = 2 + p.DissectLine(parser, line, 2, result);

                if (offset < 4) {
                    return -1;
                }

                return offset;
            }

            return -1;
        }
    }
}

#endif