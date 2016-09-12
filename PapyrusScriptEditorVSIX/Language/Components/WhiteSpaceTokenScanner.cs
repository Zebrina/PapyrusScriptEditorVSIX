using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Papyrus.Common.Extensions;

namespace Papyrus.Language.Components {
    public class WhiteSpaceTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                string whiteSpace = new string(text.Substring(offset).TakeWhile(c => Char.IsWhiteSpace(c)).ToArray());
                if (whiteSpace.Length > 0) {
                    token = new Token(new WhiteSpace(whiteSpace), sourceSnapshotSpan.Subspan(offset, whiteSpace.Length));
                    return true;
                }
            }

            token = null;
            return false;
        }
    }
}
