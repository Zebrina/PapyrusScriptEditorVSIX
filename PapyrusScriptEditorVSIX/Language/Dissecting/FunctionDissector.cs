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
    public class FunctionDissector : ILineDissector {
        private readonly string returnTypeKey;
        private readonly string parametersKey;
        private readonly string attributesKey;

        [DebuggerStepThrough]
        public FunctionDissector(string returnTypeKey, string parametersKey, string attributesKey) {
            Debug.Assert(!String.IsNullOrWhiteSpace(returnTypeKey), "'returnTypeKey' can't be null or empty.");
            Debug.Assert(!String.IsNullOrWhiteSpace(parametersKey), "'parametersKey' can't be null or empty.");
            Debug.Assert(!String.IsNullOrWhiteSpace(attributesKey), "'attributesKey' can't be null or empty.");

            this.returnTypeKey = returnTypeKey;
            this.parametersKey = parametersKey;
            this.attributesKey = attributesKey;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position == 0 && line.Count >= 4) {
                TokenType returnType = Dissecting.ExtractVariableType(parser, line, 0);

                if (!TokenType.IsNullOrInvalid(returnType)) {
                    position += returnType.Size;
                }

                if (TokenType.Equals(line[position], Keyword.Function) && line[position + 1].TypeID == TokenTypeID.Identifier) {
                    result.AddEntry(Keyword.Function, line[position + 1]);
                    position += 2;

                    ParameterListDissector p = new ParameterListDissector(parametersKey);
                    position += p.DissectLine(parser, line, position, result);

                    if (position >= 4) {
                        AttributesDissector a = new AttributesDissector(attributesKey, Keyword.Global, Keyword.Native);
                        position += a.DissectLine(parser, line, position, result);

                        return position;
                    }
                }
            }

            return -1;
        }
    }
}

#endif