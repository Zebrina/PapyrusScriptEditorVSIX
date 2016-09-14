using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Papyrus.Language.Components;
using Papyrus.Common.Extensions;

namespace Papyrus.Language {
    public class NumericLiteralTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                int length = Delimiter.FindNext(text, offset) - offset;
                NumericLiteral numericLiteral = NumericLiteral.Parse(text.Substring(offset, length));
                if (numericLiteral != null) {
                    token = new Token(numericLiteral, sourceSnapshotSpan.Subspan(offset, length));
                    return true;
                }
            }

            token = null;
            return false;
        }
    }
}
