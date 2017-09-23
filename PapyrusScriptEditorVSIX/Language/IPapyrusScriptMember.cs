using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public interface IPapyrusScriptMember : IPrintableComponent {
        PapyrusScriptMemberType Type { get; }
        string ScriptName { get; }
        IEnumerable<IPapyrusScriptMember> GetMembers(string filter);
        bool AddMember(IPapyrusScriptMember member);
        bool RemoveMember(string memberName);
    }
}
