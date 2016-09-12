#if false
using Microsoft.VisualStudio.Text;
using Papyrus.Common.Extensions;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class DelimiterTokenScanner : TokenScannerModule {
        protected override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, IReadOnlyCollection<Token> previousTokens, out Token token) {
            Delimiter delimiter = Delimiter.Parse(sourceSnapshotSpan.GetText()[offset]);
            if (delimiter != null) {
                token = new Token(delimiter, sourceSnapshotSpan.Subspan(offset, Delimiter.TokenSize));
                return true;
            }

            token = null;
            return false;
        }
    }
}

#endif