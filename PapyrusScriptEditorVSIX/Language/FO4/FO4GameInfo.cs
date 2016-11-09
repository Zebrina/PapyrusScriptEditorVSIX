using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    internal struct FO4GameInfo : IGameInfo {
        private static string[] sourceFolders = new string[] {
        };

        public string FullName {
            get { return "Fallout 4"; }
        }
        public string CompilerFlags {
            get { return "Institute_Papyrus_Flags.flg"; }
        }
        public string[] DefaultSourceFolders {
            get { return sourceFolders; }
        }

        public string FindFileInSourceFolders(string fileName) {
            return PapyrusFile.FindInSourceFolders(fileName, sourceFolders);
        }
    }
}
