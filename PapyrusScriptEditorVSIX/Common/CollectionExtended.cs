using System;
using System.Collections.Generic;

namespace Papyrus.Common {
    internal static class CollectionExtended {
        public static IEnumerable<TResult> Combine<T1, T2, TResult>(this IEnumerable<T1> firstCollection, IEnumerable<T2> secondCollection, Func<T1, T2, TResult> combiner) {
            List<TResult> result = new List<TResult>();

            using (var firstItem = firstCollection.GetEnumerator())
            using (var secondItem = secondCollection.GetEnumerator()) {
                while (firstItem.MoveNext() && secondItem.MoveNext()) {
                    result.Add(combiner.Invoke(firstItem.Current, secondItem.Current));
                }
            }

            return result;
        }
        public static IEnumerable<TResult> Combine<T1, T2, T3, TResult>(this IEnumerable<T1> firstCollection, IEnumerable<T2> secondCollection, IEnumerable<T3> thirdCollection, Func<T1, T2, T3, TResult> combiner) {
            List<TResult> result = new List<TResult>();

            using (var firstItem = firstCollection.GetEnumerator())
            using (var secondItem = secondCollection.GetEnumerator())
            using (var thirdItem = thirdCollection.GetEnumerator()) {
                while (firstItem.MoveNext() && secondItem.MoveNext() && thirdItem.MoveNext()) {
                    result.Add(combiner.Invoke(firstItem.Current, secondItem.Current, thirdItem.Current));
                }
            }

            return result;
        }
    }
}
