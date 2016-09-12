using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Papyrus.Language.Components;
using Papyrus.Common.Extensions;

namespace Papyrus.Language {
    public class StringLiteralTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                if (text[offset] == (char)Delimiter.QuotationMark) {
                    int length = Delimiter.FindNext(text, offset + 1, (char)Delimiter.QuotationMark) - offset;
                    token = new Token(new StringLiteral(text.Substring(offset + 1, length)), sourceSnapshotSpan.Subspan(offset, length + 1));
                    return true;
                }
            }

            token = null;
            return false;
        }
    }
}
