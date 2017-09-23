using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW.Parsing.Interfaces {
    public interface ITokenParser {
        bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, out IPapyrusToken token);
    }
}
