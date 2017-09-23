using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Common.Extensions {
    [DebuggerStepThrough]
    public static class StringExtended {
        public static char FirstOrNull(this string source) {
            if (source.Length == 0) {
                return '\0';
            }
            return source.First();
        }
        public static char LastOrNull(this string source) {
            if (source.Length == 0) {
                return '\0';
            }
            return source.Last();
        }

        public static string TryRemove(this string source, int startIndex) {
            if (startIndex >= 0 && startIndex < source.Length) {
                return source.Remove(startIndex);
            }
            return source;
        }
        public static string TryRemove(this string source, int startIndex, int count) {
            if (startIndex >= 0 && count >= 0 && startIndex + count < source.Length) {
                return source.Remove(startIndex, count);
            }
            return source;
        }

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
