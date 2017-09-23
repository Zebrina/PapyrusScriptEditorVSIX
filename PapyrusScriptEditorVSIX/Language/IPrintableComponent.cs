using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public interface IPrintableComponent {
        bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo);
    }
}
