﻿using Microsoft.VisualStudio.Text;

namespace Papyrus.Language {
    public sealed class TokenInfo {
        public Token Type { get; set; }
        public SnapshotSpan Span { get; set; }

        public TokenInfo(Token type, SnapshotSpan span) {
            this.Type = type;
            this.Span = span;
        }
        public TokenInfo() :
            this(null, default(SnapshotSpan)) {
        }
    }
}
