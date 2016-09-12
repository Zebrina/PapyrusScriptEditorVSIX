using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    /// <summary>
    /// Represents a script object member. This can be a field, property, function or event.
    /// </summary>
    public interface IScriptMember : IComparable<IScriptMember> {
        string Name { get; }
        string GetDeclaration();
        string GetDefinition();
        bool ChildAccessible { get; }
        IEnumerable<Parameter> Parameters { get; }
        bool Hidden { get; }
    }
}
