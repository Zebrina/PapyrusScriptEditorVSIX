using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public class Token {
        public static readonly Token Null = new Token(null, new SnapshotSpan());

        private TokenType type;
        private SnapshotSpan span;

        public Token(TokenType type, SnapshotSpan span) {
            this.type = type;
            this.span = span;
        }

        public TokenType Type {
            get { return type; }
        }
        public SnapshotSpan Span {
            get { return span; }
        }
    }
}
