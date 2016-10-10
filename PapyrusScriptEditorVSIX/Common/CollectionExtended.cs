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

        public static void SelectWhere<T, TResult>(this IEnumerable<T> collection, Predicate<T> pred, Func<T, TResult> selector, ICollection<TResult> outContainer) {
            foreach (var item in collection) {
                if (pred.Invoke(item)) {
                    outContainer.Add(selector.Invoke(item));
                }
            }
        }
        public static IEnumerable<TResult> SelectWhere<T, TResult, TResultContainer>(this IEnumerable<T> collection, Predicate<T> pred, Func<T, TResult> selector) where TResultContainer : ICollection<TResult>, new() {
            var container = new TResultContainer();
            collection.SelectWhere(pred, selector, container);
            return container;
        }
        public static IEnumerable<TResult> SelectWhere<T, TResult>(this IEnumerable<T> collection, Predicate<T> pred, Func<T, TResult> selector) {
            return collection.SelectWhere<T, TResult, List<TResult>>(pred, selector);
        }
    }
}
