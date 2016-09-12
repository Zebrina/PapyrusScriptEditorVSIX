using Papyrus.Language.Parsing;

namespace Papyrus.Language.Dissecting {
    public interface ILineDissector {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="position"></param>
        /// <param name="result"></param>
        /// <returns>Less than zero to indicate failure and zero or more to indicate the number of tokens which have been dissected.</returns>
        int DissectLine(ScriptParser parser, IReadOnlyParsedLine line, int position, DissectedLine result);
    }
}
