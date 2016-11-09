using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    internal struct TESVGameInfo : IGameInfo {
        private static string[] sourceFolders = new string[] {
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source",
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Dawnguard",
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Dragonborn",
        };

        public string FullName {
            get { return "The Elder Scrolls V: Skyrim"; }
        }
        public string CompilerFlags {
            get { return "TESV_Papyrus_Flags.flg"; }
        }
        public string[] DefaultSourceFolders {
            get { return sourceFolders; }
        }

        public string FindFileInSourceFolders(string fileName) {
            

            return PapyrusFile.FindInSourceFolders(fileName, sourceFolders);
        }
    }
}
