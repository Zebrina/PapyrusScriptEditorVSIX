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
            if (parent == null) throw new ArgumentNullException("parent");

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
                output.Activate();
            }
        }
        public void Hide() {
            var output = Output;
            if (output != null) {
                output.Hide();
            }
        }

        public void Print(string message) {
            if (!String.IsNullOrEmpty(message)) {
                var output = Output;
                if (output != null) {
                    output.OutputStringThreadSafe(message);
                }
            }
        }
        public void PrintLine(string message) {
            if (message != null) {
                Print(message);
            }
            Print(Environment.NewLine);
        }
        public void PrintFormat(string format, params object[] args) {
            Print(String.Format(format, args));
        }
        public void PrintError(string format, params object[] args) {
            if (String.IsNullOrWhiteSpace(format)) throw new ArgumentNullException("format");

            PrintFormat(format, args);
        }

        public void Clear() {
            var output = Output;
            if (output != null) {
                output.Clear();
            }
        }
    }
}
