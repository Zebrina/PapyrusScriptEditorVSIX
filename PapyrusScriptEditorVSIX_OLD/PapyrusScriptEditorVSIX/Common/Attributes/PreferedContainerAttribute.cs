using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common.Attributes {
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    internal class PreferedContainerAttribute : Attribute {
    }
}
