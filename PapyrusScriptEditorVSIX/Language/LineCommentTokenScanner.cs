using Microsoft.VisualStudio.Text;
using Papyrus.Common.Extensions;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class LineCommentTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                string text ? sourceSnapshotSpan.GetText();
                if (text[offset] == (char)Delimiter.SemiColon) {
                    token = new Token(new Comment(text.Substring(offset), false), sourceSnapshotSpan.Subspan(offset));
                    return true;
                }
            }
            
            token = null;
            return false;
        }
    }
}
