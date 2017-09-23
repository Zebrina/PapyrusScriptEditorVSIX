using Papyrus.Language_NEW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW {
    public interface IPrintableComponent {
        bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo);
    }
}
