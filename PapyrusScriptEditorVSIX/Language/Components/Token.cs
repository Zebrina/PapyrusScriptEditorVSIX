using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public struct Token {
        public static readonly Token Null = new Token(null, new SnapshotSpan());

        public TokenType Type { get; set; }
        public SnapshotSpan Span { get; set; }

        public Token(TokenType type, SnapshotSpan span) {
            this.Type = type;
            this.Span = span;
        }
    }
}
