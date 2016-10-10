using System;
using System.ComponentModel;
using System.Linq;

namespace Papyrus.Common {
    public static class EnumExtended {
        /// <summary>
        /// http://stackoverflow.com/questions/1799370/getting-attributes-of-enums-value
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value, Type type) {
            return ((DescriptionAttribute)(type.GetMember(value.ToString()).First().GetCustomAttributes(typeof(DescriptionAttribute), false).First())).Description;
        }
    }
}
