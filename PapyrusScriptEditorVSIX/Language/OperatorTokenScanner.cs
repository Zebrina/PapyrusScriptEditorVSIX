using Microsoft.VisualStudio.Text;
using Papyrus.Common.Extensions;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class OperatorTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            Operator op = Operator.Parse(sourceSnapshotSpan.GetText(), offset);
            if (op != null) {
                token = new Token(op, sourceSnapshotSpan.Subspan(offset));
            }

            token = null;
            return false;
        }
    }
}
