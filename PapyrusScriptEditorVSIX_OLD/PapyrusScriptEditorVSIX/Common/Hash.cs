using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common {
    internal static class Hash {
        public static int GetMemberwiseHashCode(params object[] objs) {
            int hash = 17;
            foreach (var obj in objs) {
                hash = hash * 23 + obj.GetHashCode();
            }
            return hash;
        }
    }
}
