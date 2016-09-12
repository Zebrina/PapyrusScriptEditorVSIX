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
    public class IdentifierTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            string text = sourceSnapshotSpan.GetText();
            int length = Delimiter.FindNext(text, offset) - offset;
            if (Identifier.IsValid(text.Substring(offset, length))) {
                token = new Token(new Identifier(text.Substring(offset, length)), sourceSnapshotSpan.Subspan(offset, length));
                return true;
            }

            token = null;
            return false;
        }
    }
}
