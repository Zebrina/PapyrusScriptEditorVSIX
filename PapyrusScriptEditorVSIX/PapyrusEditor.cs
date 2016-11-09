using Papyrus.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus {
    public class PapyrusEditor {
        private static PapyrusEditor instance = null;
        private static object instanceLock = new object();
        public static PapyrusEditor Instance {
            get {
                if (instance == null) {
                    lock (instanceLock) {
                        if (instance == null) {
                            instance = new PapyrusEditor();
                        }
                    }
                }
                return instance;
            }
        }

        public IGameInfo CurrentGame { get; set; }
        public string OutputFolder { get; set; }

        private PapyrusEditor() {
            CurrentGame = new TESVGameInfo();
            OutputFolder = "C:\\PapyrusScripts";
        }
    }
}
