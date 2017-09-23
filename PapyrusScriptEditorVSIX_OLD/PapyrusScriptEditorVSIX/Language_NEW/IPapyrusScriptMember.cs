using Papyrus.Language_NEW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW {
    public interface IPapyrusScriptMember : IPrintableComponent {
        PapyrusScriptMemberType Type { get; }
        string Name { get; }
        IEnumerable<IPapyrusScriptMember> GetMembers(string filter);
        bool AddMember(IPapyrusScriptMember member);
        bool RemoveMember(string memberName);
    }
}
