using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using Papyrus.Common.Extensions;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class BlockCommentTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            string text = sourceSnapshotSpan.GetText();
            int endOffset, length;
            if (state == TokenScannerState.Text) {
                if (String.Compare(text, offset, Comment.BlockBegin, 0, Comment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                    endOffset = text.IndexOf(Comment.BlockEnd, offset + Comment.BlockBegin.Length);
                    if (endOffset == -1) {
                        state = TokenScannerState.BlockComment;
                        token = new Token(new Comment(text.Substring(offset), true), sourceSnapshotSpan.Subspan(offset));
                    }
                    else {
                        length = (endOffset + Comment.BlockEnd.Length) - offset;
                        token = new Token(new Comment(text.Substring(offset, length), true), sourceSnapshotSpan.Subspan(offset, length));
                    }
                    return true;
                }
            }
            else if (state == TokenScannerState.BlockComment) {
                endOffset = text.IndexOf(Comment.BlockEnd, offset);
                if (endOffset == -1) {
                    token = new Token(new Comment(text.Substring(offset), true), sourceSnapshotSpan.Subspan(offset));
                }
                else {
                    state = TokenScannerState.Text;
                    length = (endOffset + Comment.BlockEnd.Length) - offset;
                    token = new Token(new Comment(text.Substring(offset, length), true), sourceSnapshotSpan.Subspan(offset, length));
                }
                return true;
            }

            token = null;
            return false;
        }
    }
}
