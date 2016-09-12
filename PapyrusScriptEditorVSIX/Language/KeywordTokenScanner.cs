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
    public class KeywordTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            string text = sourceSnapshotSpan.GetText();
            Keyword keyword = Keyword.Parse(text, offset, Delimiter.FindNext(text, offset) - offset);
            if (keyword != null) {
                token = new Token(keyword, new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(offset, keyword.Text.Length)));
                return true;
            }

            token = null;
            return false;
        }
    }
}
