using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Papyrus.Games;
using Papyrus.Language;
using Papyrus.Language.ScriptMembers;
using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus {
    public class TargetGameInfoChangedEventArgs : EventArgs {
        public IGameInfo OldGameInfo { get; private set; }
        public IGameInfo NewGameInfo { get; private set; }

        internal TargetGameInfoChangedEventArgs(IGameInfo oldGameInfo, IGameInfo newGameInfo) {
            this.OldGameInfo = oldGameInfo;
            this.NewGameInfo = newGameInfo;
        }
    }

    public static class PapyrusEditor {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class RegisterGameInfoAttribute : Attribute {
        }
        
        public delegate void OnTargetGameInfoChanged(object sender, TargetGameInfoChangedEventArgs e);
        
        public static event OnTargetGameInfoChanged TargetGameInfoChanged;

        private static Package package = null;
        private static IGameInfo activeGame = null;
        private static List<IGameInfo> registeredGames = null;
        private static List<string> additionalImportFolders = new List<string>();

        private static Dictionary<Window, PapyrusScript> editorActiveScripts = new Dictionary<Window, PapyrusScript>();

        internal static void Initialize(Package package) {
            PapyrusEditor.package = package;

            OutputFolder = "C:\\PapyrusScripts";

            _DTE.Events.WindowEvents.WindowCreated += WindowEvents_WindowCreated;
            _DTE.Events.WindowEvents.WindowClosing += WindowEvents_WindowClosing;
        }

        private static void WindowEvents_WindowCreated(Window window) {
            string documentName = window.Document.Name;
            if (documentName.EndsWith(".psc", StringComparison.OrdinalIgnoreCase)) {
                PapyrusScript papyrusScript = new PapyrusScript(documentName.Remove(documentName.Length - 4));
                PapyrusScriptManager.Instance.AddScript(papyrusScript);
                editorActiveScripts.Add(window, papyrusScript);
            }
            
        }
        private static void WindowEvents_WindowClosing(Window window) {
            editorActiveScripts.Remove(window);
        }

        public static IServiceProvider ServiceProvider {
            get { return package; }
        }
        private static DTE _DTE {
            get { return (DTE)ServiceProvider.GetService(typeof(DTE)); }
        }
        public static IGameInfo ActiveGame {
            get {
                if (activeGame == null && RegisteredGames.Count > 0) {
                    activeGame = RegisteredGames.First();
                }
                return activeGame;
            }
            set {
                if (value != activeGame) {
                    var e = new TargetGameInfoChangedEventArgs(activeGame, value);
                    activeGame = value;
                    TargetGameInfoChanged?.Invoke(null, e);
                }
            }
        }
        public static IReadOnlyList<IGameInfo> RegisteredGames {
            get {
                if (registeredGames == null) {
                    registeredGames = new List<IGameInfo>();

                    foreach (Type t in Assembly.GetCallingAssembly().GetExportedTypes().Where(t => t.IsDefined(typeof(RegisterGameInfoAttribute), false))) {
                        if (t.GetInterface("IGameInfo") == null) {
                            continue;
                        }

                        ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor == null) {
                            continue;
                        }

                        registeredGames.Add((IGameInfo)ctor.Invoke(null));
                    }
                }
                return registeredGames;
            }
        }

        public static PapyrusScript ActivePapyrusScript {
            get {
                PapyrusScript papyrusScript;
                if (editorActiveScripts.TryGetValue(_DTE.ActiveWindow, out papyrusScript)) {
                    return papyrusScript;
                }
                return null;
            }
        }

        public static string OutputFolder { get; set; }
        public static List<string> AdditionalImportFolders {
            get { return additionalImportFolders; }
        }
        public static bool IsReady {
            get { return ActiveGame != null; }
        }

        public static string FindSourceFile(string scriptName) {
            if (ActiveGame == null) {
                return null;
            }

            List<string> importFolders = new List<string>();

            importFolders.Add(Path.GetDirectoryName(_DTE.ActiveDocument.FullName));
            importFolders.AddRange(additionalImportFolders);
            importFolders.AddRange(ActiveGame.DefaultSourceFolders);

            foreach (var folder in importFolders) {
                string filePath = String.Format("{0}\\{1}.psc", folder, scriptName);
                if (File.Exists(filePath)) {
                    return filePath;
                }
            }
            return null;
        }
    }
}
