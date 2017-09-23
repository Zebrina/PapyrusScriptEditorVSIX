using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public class TokenManager<T> : IEnumerable<T> where T : class {
        private Dictionary<string, T> tokenCollection;

        public TokenManager(bool allowStaticTokens) {
            tokenCollection = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

            if (allowStaticTokens) {
                Type type = typeof(T);

                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                    var token = field.GetValue(null);
                    if (token != null) {
                        StaticTokenAttribute attribute = (StaticTokenAttribute)field.GetCustomAttribute(typeof(StaticTokenAttribute));
                        if (attribute != null) {
                            tokenCollection.Add(field.Name, token as T);
                        }
                    }
                }
            }
        }

        public T ParseToken(string tokenStr) {
            T token;
            if (tokenCollection.TryGetValue(tokenStr, out token)) {
                var field = typeof(T).GetFields().SingleOrDefault(f => String.Equals(f.Name, tokenStr, StringComparison.OrdinalIgnoreCase));
                if (field != null) {
                    StaticTokenAttribute attribute = (StaticTokenAttribute)field.GetCustomAttribute(typeof(StaticTokenAttribute));
                    if (attribute == null || attribute.AllowedInGame(PapyrusEditor.ActiveGame)) {
                        return token;
                    }
                }

            }
            return null;
        }

        public bool AddDynamicToken(string key, T token) {
            if (!tokenCollection.ContainsKey(key)) {
                tokenCollection.Add(key, token);
                return true;
            }
            return false;
        }
        public bool RemoveDynamicToken(string key) {
            return tokenCollection.Remove(key);
        }

        public IEnumerator<T> GetEnumerator() {
            return tokenCollection.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return tokenCollection.Values.GetEnumerator();
        }
    }
}
