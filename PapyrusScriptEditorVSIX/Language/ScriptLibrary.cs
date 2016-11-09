using Microsoft.VisualStudio.Shell;
using Papyrus.Common;
using Papyrus.Language.Components.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Papyrus.Language {
    [Obsolete]
    /// <summary>
    /// Implemented as singleton.
    /// </summary>
    public sealed class ScriptLibrary : IDisposable {
        private Package package;
        private ICollection<string> sourceFolders;
        private List<FileSystemWatcher> fileSystemWatchers;

        public ScriptLibrary() {
            sourceFolders = new List<string>() {
                @"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source",
                /*@"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Dawnguard",*/
                /*@"C:\Games\Steam\SteamApps\common\Skyrim\Data\Scripts\Source\Dragonborn",*/
            };

            fileSystemWatchers = new List<FileSystemWatcher>();

            // Create one FileSystemWatcher for each source folder.
            foreach (string folder in sourceFolders) {
                FileSystemWatcher watcher = new FileSystemWatcher();

                watcher.Path = folder;

                // We are only interested in papyrus source files.
                watcher.Filter = PapyrusContentDefinition.FileExtension;

                // File system events.
                watcher.Changed += FileSystemWatcher_Changed;
                watcher.Created += FileSystemWatcher_Created;
                watcher.Deleted += FileSystemWatcher_Deleted;
                watcher.Renamed += FileSystemWatcher_Renamed;

                watcher.EnableRaisingEvents = true;

                fileSystemWatchers.Add(watcher);
            }
        }

        public void Initialize(Package package) {
            this.package = package;

            //ScriptParser parser = new ScriptParser(Instance.sourceFolders);
            //parser.ParseAllScripts();

            //StreamWriter log = new StreamWriter(new FileStream(@"C:\VSPapyrus.log", FileMode.Create, FileAccess.Write, FileShare.Read));
            //Instance.Dump(log);
            //log.Close();
        }

        /*
        public IEnumerable<string> SourceFolders {
            get { return sourceFolders; }
        }
        */

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
            if (File.Exists(e.FullPath)) {
                //ScriptParser parser = new ScriptParser(Instance.sourceFolders);
                //parser.ParseScript(e.FullPath, true);
                MessageBox.Show("REPARSED A FILE");
            }
            else {
                MessageBox.Show(e.FullPath + " is not a file!");
            }
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e) {
            if (File.Exists(e.FullPath)) {
                //ScriptParser parser = new ScriptParser(Instance.sourceFolders);
                //parser.ParseScript(e.FullPath);
                MessageBox.Show("REPARSED A FILE");
            }
            else {
                MessageBox.Show(e.FullPath + " is not a file!");
            }
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e) {
            //throw new NotImplementedException();
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e) {
            //throw new NotImplementedException();
        }

        public string FindFileInSourceFolders(string fileName) {
            foreach (var folder in sourceFolders) {
                string filePath = String.Format("{0}\\{1}.psc", folder, fileName);
                if (File.Exists(filePath)) {
                    return filePath;
                }
            }
            return null;
        }

        [Conditional("DEBUG")]
        //[DebuggerStepThrough]
        internal void Dump(TextWriter stream) {
            stream.WriteLine(DateTime.Today);
            stream.WriteLine();

            foreach (ScriptObject script in ScriptObject.Manager.ScriptObjects) {
                stream.WriteLine(script.ToString());
                stream.WriteLine();
            }
        }

        public void Dispose() {
            fileSystemWatchers.Clear();
        }
    }
}
