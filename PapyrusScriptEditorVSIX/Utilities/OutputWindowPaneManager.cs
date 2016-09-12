using Microsoft.VisualStudio.Shell;
using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Utilities {
    public class OutputWindowPaneManager : Singleton<OutputWindowPaneManager> {
        private Package package = null;

        internal void Initialize(Package package) {
            if (package == null) {
                throw new ArgumentNullException("package");
            }

            this.package = package;
        }

        public Package Package { get { return package; } }

        public OutputWindowPane CreateWindowPane(Guid guid, string caption) {
            return new OutputWindowPane(this, guid, caption);
        }
    }
}
