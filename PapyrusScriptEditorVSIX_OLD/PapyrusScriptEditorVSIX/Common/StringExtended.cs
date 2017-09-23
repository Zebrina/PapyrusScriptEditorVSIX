using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Common {
    [DebuggerStepThrough]
    public static class StringExtended {
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

        public static bool ContainsIgnoreCase(this string source, string value) {
            return source.ToLowerInvariant().Contains(value.ToLowerInvariant());
        }
    }
}
