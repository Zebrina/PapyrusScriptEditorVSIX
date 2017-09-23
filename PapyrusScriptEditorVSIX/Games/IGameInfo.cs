using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Games {
    public interface IGameInfo {
        string CompilerFlags { get; }
        string[] DefaultSourceFolders { get; }
    }
}
