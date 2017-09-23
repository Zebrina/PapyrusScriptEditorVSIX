using Papyrus.Language_NEW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW {
    /// <summary>
    /// All implementations should be non-nullable. Return NullToken.Singleton instead of null.
    /// </summary>
    public interface IPapyrusToken : IPrintableComponent {
        PapyrusTokenType Type { get; }
        /// <summary>
        /// Returns the number of characters this token encompasses.
        /// </summary>
        int TokenSize { get; }
        /// <summary>
        /// Returns true for tokens that are considered constant values, e.g. true, false, string and numeric literals.
        /// </summary>
        bool IsCompileTimeConstant { get; }
        /// <summary>
        /// Returns true if the token wants the parser to treat the next line as part of this line. Only the token that represents '\' should return true here.
        /// </summary>
        bool IsLineExtension { get; }
        /// <summary>
        /// Returns true if this token is to be ignored by the parser, i.e. be discarded after parse.
        /// </summary>
        bool IsIgnoredByParser { get; }
        bool IsEqualToToken(IPapyrusToken other);
    }
}
