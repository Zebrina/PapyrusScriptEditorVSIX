using System.Text;

namespace Papyrus.Common {
    internal static class StringBuilderExtended {
        public static void AppendWhiteSpace(this StringBuilder builder) {
            builder.Append(' ');
        }
    }
}
