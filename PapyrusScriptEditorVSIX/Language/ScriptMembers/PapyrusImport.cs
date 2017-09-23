using Papyrus.Language.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.ScriptMembers {
    public sealed class PapyrusImport {
        private PapyrusScript importedScript;

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            stringBuilder.Append(PapyrusKeyword.CustomEvent);
            stringBuilder.Append(' ');
            stringBuilder.Append(importedScript.ScriptName);

            return true;
        }
    }
}
