using Microsoft.VisualStudio.Shell;
using Papyrus.Common;
using System;

namespace Papyrus.Utilities {
    public class OutputWindowPaneManager {
        private static OutputWindowPaneManager instance = null;

        public static OutputWindowPaneManager Instance {
            get { return instance; }
        }
        internal static void Initialize(Package package) {
            if (package == null) {
                throw new ArgumentNullException("package");
            }

            instance = new OutputWindowPaneManager(package);
        }

        private Package package = null;

        private OutputWindowPaneManager(Package package) {
            this.package = package;
        }

        public Package Package {
            get { return package; }
        }

        public OutputWindowPane CreateWindowPane(Guid guid, string caption) {
            return new OutputWindowPane(this, guid, caption);
        }
    }
}
