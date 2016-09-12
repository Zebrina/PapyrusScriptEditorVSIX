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
    public class ParameterDissector : ILineDissector {
        private readonly string key;

        [DebuggerStepThrough]
        public ParameterDissector(string key) {
            Debug.Assert(!String.IsNullOrWhiteSpace(key), "'key' can't be null or empty.");

            this.key = key;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position + 2 < line.Count) {
                TokenType type = Dissecting.ExtractVariableType(parser, line, position);

                if (!TokenType.IsNullOrInvalid(type)) {
                    int offset = type.Size;
                    if (line[position + offset].TypeID == TokenTypeID.Identifier) {
                        // A valid parameter so far.

                        string name = line[position + offset].Text;
                        offset += 1;

                        Parameter parameter;

                        // Check for default value.
                        if (TokenType.Equals(line[position + offset], Operator.BasicAssignment) && line[position + offset + 1].CompileTimeConstant) {
                            parameter = new Parameter(type, name, line[position + offset + 1]);
                            offset += 2;
                        }
                        else {
                            parameter = new Parameter(type, name);
                        }

                        result.AddArrayEntry(key, parameter);
                        return offset;
                    }
                }
            }

            return -1;
        }
    }
}

#endif