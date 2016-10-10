using System;

namespace Papyrus.Common {
    public class Proxy<T> where T : class {
        private T instance;
        private Func<T> allocator;

        public Proxy(Func<T> allocator) {
            this.instance = null;
            this.allocator = allocator;
        }

        public static implicit operator T(Proxy<T> proxy) {
            if (proxy.instance == null) {
                proxy.instance = proxy.allocator.Invoke();
            }
            return proxy.instance;
        }
    }
}
