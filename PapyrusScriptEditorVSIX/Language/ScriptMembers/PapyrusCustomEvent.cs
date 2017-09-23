using Papyrus.Language.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.ScriptMembers {
    /// <summary>
    /// A custom event declaration. Fallout 4 only.
    /// </summary>
    public sealed class PapyrusCustomEvent {
        public string Name { get; private set; }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            stringBuilder.Append(PapyrusKeyword.CustomEvent);
            stringBuilder.Append(' ');
            stringBuilder.Append(Name);

            return false;
        }
    }
}
