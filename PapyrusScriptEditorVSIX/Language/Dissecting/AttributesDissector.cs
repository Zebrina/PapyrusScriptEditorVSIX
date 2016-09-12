#if false
using System.Collections;
using System.Collections.Generic;
using Papyrus.Language.Parsing;
using Papyrus.Language.Components;
using System.Diagnostics;
using System;

namespace Papyrus.Language.Dissecting {
    public class AttributesDissector : ILineDissector {
        private readonly string key;
        private readonly IReadOnlyCollection<TokenType> validAttributes;

        [DebuggerStepThrough]
        public AttributesDissector(string key, TokenType validAttribute, params TokenType[] additionalValidAttributes) {
            Debug.Assert(!String.IsNullOrWhiteSpace(key), "'key' can't be null or empty.");

            this.key = key;
            TokenType[] array = new TokenType[1 + additionalValidAttributes.Length];
            array[0] = validAttribute;
            additionalValidAttributes.CopyTo(array, 1);
            this.validAttributes = array;
        }

        public int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result) {
            HashSet<TokenType> attributeSet = new HashSet<TokenType>(this.validAttributes);

            TokenType token;
            int offset = 0;
            while (attributeSet.Contains(token = line[position + offset])) {
                attributeSet.Remove(token);

                result.AddArrayEntry(key, token);

                ++offset;
            }

            return offset;
        }
    }
}

#endif