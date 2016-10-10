using System.Collections.Generic;

namespace Papyrus.Language {
    public interface ISyntaxParsable {
        bool TryParse(IReadOnlyList<Token> tokens, int offset);
        int Length { get; }
    }
}
