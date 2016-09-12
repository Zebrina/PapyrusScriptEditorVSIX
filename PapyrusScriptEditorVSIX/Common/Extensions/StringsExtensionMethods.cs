using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common.Extensions {
    [DebuggerStepThrough]
    public static class StringsExtended {
        /// <summary>
        /// Retrieves a span in this string. The span starts at a specified character position and ends at a later specified character position.
        /// </summary>
        /// <param name="startIndex">Index of first character.</param>
        /// <param name="endIndex">Index of last last character. Inclusive.</param>
        public static string Span(this string source, int startIndex, int endIndex) {
            return source.Substring(startIndex, (endIndex + 1) - startIndex);
        }

        public static int IndexOfNextToken(this string source, char token, int startIndex) {
            int index = source.IndexOf(token, startIndex);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfNextToken(this string source, char token) {
            int index = source.IndexOf(token, 0);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfNextToken(this string source, string token, int startIndex) {
            int index = source.IndexOf(token, startIndex);
            return index == -1 ? source.Length : index;
        }
        public static int IndexOfNextToken(this string source, string token) {
            int index = source.IndexOf(token, 0);
            return index == -1 ? source.Length : index;
        }
    }
}
