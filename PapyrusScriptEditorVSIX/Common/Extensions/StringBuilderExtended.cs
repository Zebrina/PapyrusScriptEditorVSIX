using System.Text;

namespace Papyrus.Common.Extensions {
    internal static class StringBuilderExtended {
        public static void AppendWhiteSpace(this StringBuilder builder) {
            builder.Append(' ');
        }
    }
}
