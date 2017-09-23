using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Papyrus {
    [Guid(PapyrusGUID.OptionPageGridGuidString)]
    internal class PapyrusOptionPage : DialogPage {
        [Category("Papyrus")]
        [DisplayName("Output Folder")]
        [Description("Output Folder")]
        public string OutputFolder {
            get { return PapyrusEditor.OutputFolder; }
            set { PapyrusEditor.OutputFolder = value; }
        }

        protected override IWin32Window Window {
            get {
                PapyrusOptionPageUserControl page = new PapyrusOptionPageUserControl(this);
                page.Initialize();
                return page;
            }
        }
    }
}
