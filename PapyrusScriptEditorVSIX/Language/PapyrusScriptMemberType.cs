using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public enum PapyrusScriptMemberType {
        PapyrusScript,
        PapyrusImport,
        PapyrusField,
        PapyrusProperty,
        PapyrusFunction,
        PapyrusEvent,
        // FO4
        PapyrusCustomEvent,
        PapyrusGroup,
        PapyrusStruct,
    }
}
