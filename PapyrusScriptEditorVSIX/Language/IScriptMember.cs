using Papyrus.Language.Components;
using System;
using System.Collections.Generic;

namespace Papyrus.Language {
    /// <summary>
    /// Represents a script object member. This can be a field, property, function or event.
    /// </summary>
    public interface IScriptMember : IComparable<IScriptMember>, ICloneable {
        string Name { get; }
        string GetDeclaration();
        string GetDefinition();
        bool IsChildAccessible { get; }
        IEnumerable<Parameter> Parameters { get; }
        bool Hidden { get; }
    }

    public interface IScriptMemberMatcher {

    }
}
