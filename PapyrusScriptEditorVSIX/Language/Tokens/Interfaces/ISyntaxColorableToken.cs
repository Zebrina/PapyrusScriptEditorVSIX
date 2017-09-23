using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Tokens.Interfaces {
    public interface ISyntaxColorableToken {
        IClassificationType GetClassificationType(IClassificationTypeRegistryService registry);
    }
}
