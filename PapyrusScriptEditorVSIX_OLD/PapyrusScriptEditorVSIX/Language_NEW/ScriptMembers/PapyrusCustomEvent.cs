using Papyrus.Language_NEW.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW.ScriptMembers {
    /// <summary>
    /// A custom event declaration. Fallout 4 only.
    /// </summary>
    public sealed class PapyrusCustomEvent {
        public PapyrusIdentifier Identifier { get; private set; }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            stringBuilder.Append(PapyrusKeyword.CustomEvent);
            return false;
        }
    }
}
