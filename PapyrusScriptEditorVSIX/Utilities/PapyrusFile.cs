using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Utilities {
    public static class PapyrusFile {
        public static string FindInSourceFolders(string fileName, IEnumerable<string> sourceFolders) {
            foreach (var folder in sourceFolders) {
                string filePath = String.Format("{0}\\{1}.psc", folder, fileName);
                if (File.Exists(filePath)) {
                    return filePath;
                }
            }
            return null;
        }
    }
}
