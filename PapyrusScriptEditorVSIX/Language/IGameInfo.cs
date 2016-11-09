using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public interface IGameInfo {
        string FullName { get; }
        string CompilerFlags { get; }
        string[] DefaultSourceFolders { get; }
        string FindFileInSourceFolders(string fileName);
    }
}
