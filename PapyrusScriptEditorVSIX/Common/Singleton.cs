using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common {
    public class Singleton<T> where T : new() {
        private static T instance;
        private static object syncRoot = new object();

        public static T Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
