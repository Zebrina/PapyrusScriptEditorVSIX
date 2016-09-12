#if false
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace Papyrus {
    [Guid(GuidString)]
    internal class PapyrusLanguageService : LanguageService {
        public const string GuidString = "25D23BEB-2F91-4471-8CE1-1CB333EB5FC0";

        public override string Name { get { return "Papyrus Language Service"; } }

        private readonly PapyrusScanner scanner = new PapyrusScanner();

        public PapyrusLanguageService() :
            base() {
        }

        public override string GetFormatFilterList() {
            return PapyrusContentDefinition.ContentType;
        }

        public override int GetItemCount(out int count) {
            count = LegacySyntaxColorization.ColorableItems.Count;
            return VSConstants.S_OK;
        }
        public override int GetColorableItem(int index, out IVsColorableItem item) {
            return LegacySyntaxColorization.ColorableItems.TryGetValue((TokenColorID)index, out item) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }


        public override LanguagePreferences GetLanguagePreferences() {
            return new LanguagePreferences();
        }

        public override IScanner GetScanner(IVsTextLines buffer) {
            scanner.SetBuffer(buffer);
            return scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest request) {
            AuthoringScope parser = new PapyrusAuthoringScope();

            return parser;
        }
    }
}

#endif