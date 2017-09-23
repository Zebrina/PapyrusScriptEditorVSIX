using Papyrus.Language;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;

namespace Papyrus.Language.Tokens {
    /// <summary>
    /// To be used in place of null.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class PapyrusNullToken : IPapyrusToken {
        public static readonly PapyrusNullToken Singleton = new PapyrusNullToken();

        private PapyrusNullToken() { }

        PapyrusTokenType IPapyrusToken.Type { get { return PapyrusTokenType.Null; } }
        int IPapyrusToken.TokenSize { get { return 0; } }
        bool IPapyrusToken.IsCompileTimeConstant { get { return false; } }
        bool IPapyrusToken.IsIgnoredByParser { get { return true; } }
        bool IPapyrusToken.IsLineExtension { get { return false; } }
    }
}
