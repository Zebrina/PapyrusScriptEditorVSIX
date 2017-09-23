using Papyrus.Language;
using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Games {
    [PapyrusEditor.RegisterGameInfo]
    public class TESVGameInfo : IGameInfo {
        private static string[] sourceFolders = new string[] {
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source",
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Dawnguard",
            @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Hearthfire",
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

        public override string ToString() {
            return FullName;
        }
    }
}
