using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common {
    internal class Unsafe {
        public unsafe static int CompareMemory(void* ptr1, void* ptr2, int size) {
            for (int i = 0; i < size; ++i) {
                int cmp = ((byte*)ptr2)[i] - ((byte*)ptr1)[i];
                if (cmp != 0) {
                    return cmp;
                }
            }
            return 0;
        }
    }
}
