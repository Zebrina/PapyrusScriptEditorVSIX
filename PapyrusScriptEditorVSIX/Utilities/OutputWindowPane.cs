using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Papyrus.Utilities {
    /// <summary>
    /// Wrapper class for IVsOutputWindowPane.
    /// </summary>
    public class OutputWindowPane {
        private OutputWindowPaneManager parent;
        private Guid guid;
        private string caption;

        public OutputWindowPane(OutputWindowPaneManager parent, Guid guid, string caption) {
            if (parent == null) {
                throw new ArgumentNullException("parent");
            }

            this.parent = parent;
            this.guid = guid;
            this.caption = caption;
        }

        private IVsOutputWindowPane Output {
            get { return parent.Package != null ? parent.Package.GetOutputPane(guid, caption) : null; }
        }

        public void Show() {
            var output = Output;
            if (output != null) {
                Output.Activate();
            }
        }
        public void Hide() {
            var output = Output;
            if (output != null) {
                Output.Hide();
            }
        }

        public void Print(string message) {
            var output = Output;
            if (output != null) {
                Output.OutputStringThreadSafe(message);
            }
        }

        public void Clear() {
            var output = Output;
            if (output != null) {
                Output.Clear();
            }
        }
    }
}
