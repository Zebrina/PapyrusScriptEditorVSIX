using System.Diagnostics;

namespace Papyrus.Common {
    [DebuggerStepThrough]
    public static class StringsExtended {
        public static int IndexOfToken(this string source, char token, int startIndex) {
            int index = source.IndexOf(token, startIndex);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfToken(this string source, char token) {
            int index = source.IndexOf(token, 0);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfToken(this string source, string token, int startIndex) {
            int index = source.IndexOf(token, startIndex);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfToken(this string source, string token) {
            int index = source.IndexOf(token, 0);
            return index == -1 ? source.Length : index;
        }
    }
}
