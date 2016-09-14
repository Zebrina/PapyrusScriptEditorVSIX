using Microsoft.VisualStudio.Text;
using Papyrus.Common.Extensions;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class ScriptObjectTokenScanner : TokenScannerModule {
        public override bool Scan(SnapshotSpan sourceSnapshotSpan, int offset, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                int nextDelimiter = Delimiter.FindNext(text, offset);
                if (nextDelimiter > offset) {
                    ScriptObject scriptObject;
                    if (ScriptObject.TryParse(text.Substring(offset, nextDelimiter - offset), out scriptObject)) {
                        token = new Token(scriptObject, new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(offset, scriptObject.Text.Length)));
                        return true;
                    }
                }
            }

            token = null;
            return false;
        }
    }
}
