using Papyrus.Language;
using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Games {
    [PapyrusEditor.RegisterGameInfo]
    public class FO4GameInfo : IGameInfo {
        private static string[] sourceFolders = new string[] {
            @"C:\Games\Steam\SteamApps\commob\Fallout 4\Data\Scripts\Source\F4SE",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\Base",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC01",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC02",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC03",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC04",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC05",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\DLC06",
            @"C:\Games\Steam\SteamApps\common\Fallout 4\Data\Scripts\Source\User",
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

        public override string ToString() {
            return FullName;
        }
    }
}
