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
    public class ParameterListDissector : ILineDissector {
        private string key;

        [DebuggerStepThrough]
        public ParameterListDissector(string key) {
            Debug.Assert(!String.IsNullOrWhiteSpace(key), "'key' can't be null or empty.");

            this.key = key;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position + 2 <= line.Count && TokenType.Equals(line[position], Delimiter.LeftRoundBracket)) {
                ParameterDissector p = new ParameterDissector(key);

                int totalOffset = 1 + Math.Max(0, p.DissectLine(parser, line, position + 1, result));
                if (totalOffset > 1) {
                    while (TokenType.Equals(line[position + totalOffset], Delimiter.Comma)) {
                        ++totalOffset;

                        int paramOffset = p.DissectLine(parser, line, position + totalOffset, result);
                        if (paramOffset == -1) {
                            // Comma, but no valid parameter!
                            return -1;
                        }

                        totalOffset += paramOffset;
                    }
                }

                if (TokenType.Equals(line[position + totalOffset], Delimiter.RightRoundBracket)) {
                    return totalOffset + 1;
                }
            }

            return -1;
        }
    }
}

#endif