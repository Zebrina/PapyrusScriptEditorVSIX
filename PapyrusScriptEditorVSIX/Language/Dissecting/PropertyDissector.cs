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
    public class PropertyDissector : ILineDissector {
        private readonly string returnTypeKey;
        private readonly string propertyTypeKey;
        private readonly string attributesKey;

        [DebuggerStepThrough]
        public PropertyDissector(string returnTypeKey, string propertyTypeKey, string attributesKey) {
            Debug.Assert(!String.IsNullOrWhiteSpace(returnTypeKey), "'returnTypeKey' can't be null or empty.");
            Debug.Assert(!String.IsNullOrWhiteSpace(propertyTypeKey), "'propertyTypeKey' can't be null or empty.");
            Debug.Assert(!String.IsNullOrWhiteSpace(attributesKey), "'attributesKey' can't be null or empty.");

            this.returnTypeKey = returnTypeKey;
            this.propertyTypeKey = propertyTypeKey;
            this.attributesKey = attributesKey;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            if (position == 0 && line.Count >= 3) {
                TokenType returnType = Dissecting.ExtractVariableType(parser, line, 0);
                position += returnType.Size;

                if (!TokenType.IsNullOrInvalid(returnType) && TokenType.Equals(line[position], Keyword.Property) && line[position + 1].TypeID == TokenTypeID.Identifier) {
                    result.AddEntry(returnTypeKey, returnType);
                    result.AddEntry(Keyword.Property, line[position + 1]);
                    position += 2;

                    if (line.Count > position) {
                        if (TokenType.Equals(line[position], Operator.BasicAssignment)) {
                            if (!line[position + 1].CompileTimeConstant) {
                                return -1;
                            }

                            result.AddEntry(Operator.BasicAssignment, line[position + 1]);
                            position += 2;
                        }

                        if (TokenType.Equals(line[position], Keyword.Auto) || TokenType.Equals(line[position], Keyword.AutoReadOnly)) {
                            result.AddEntry(propertyTypeKey, line[position]);
                            position += 1;
                        }

                        AttributesDissector a = new AttributesDissector(attributesKey, Keyword.Conditional, Keyword.Hidden);
                        position += a.DissectLine(parser, line, position, result);
                    }

                    return position;
                }
            }

            return -1;
        }
    }
}

#endif