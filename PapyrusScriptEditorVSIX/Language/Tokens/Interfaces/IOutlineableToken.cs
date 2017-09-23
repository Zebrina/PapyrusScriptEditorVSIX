using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Tokens {
    public interface IOutlineableToken {
        bool IsOutlineableStart(IEnumerable<PapyrusTokenInfo> line);
        bool IsOutlineableEnd(IOutlineableToken startToken);
        bool IsImplementation { get; }
        string CollapsedText { get; }
        bool CollapseFirstLine { get; }
    }
}
