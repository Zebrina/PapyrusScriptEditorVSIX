using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common {
    internal interface IKeyByValue<T> {
        T Key { get; }
    }

    internal static class DictionaryExtended {
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value) where TValue : IKeyByValue<TKey> {
            dictionary.Add(value.Key, value);
        }
    }
}
