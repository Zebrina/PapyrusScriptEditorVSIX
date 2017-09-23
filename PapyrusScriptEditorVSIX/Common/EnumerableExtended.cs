using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common {
    public enum IterationResult {
        Continue,
        Success,
        Fail,
    }

    public static class EnumerableExtended {
        public static bool FindPattern<TElement>(this IEnumerable<TElement> enumerable, bool defaultResult, Func<TElement, int, IterationResult> func) {
            int index = 0;
            foreach (var e in enumerable) {
                IterationResult result = func.Invoke(e, index++);
                switch (result) {
                    case IterationResult.Success: return true;
                    case IterationResult.Fail: return false;
                }
            }
            return defaultResult;
        }
        public static bool FindPattern<TElement>(this IEnumerable<TElement> enumerable, Func<TElement, int, IterationResult> func) {
            return enumerable.FindPattern(true, func);
        }
    }
}
