using Microsoft.VisualStudio.Shell;
using Papyrus.Language;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Papyrus.Common;

namespace Papyrus.Utilities {
    /*
     *  <summary>
     *  Usage:
     *  PapyrusCompiler <object or folder> [<arguments>]
     *  
     *    object     Specifies the object to compile. (-all is not specified)
     *    folder     Specifies the folder to compile. (-all is specified)
     *    arguments  One or more of the following:
     *     -debug|d
     *      Turns on compiler debugging, outputting dev information to the screen.
     *     -optimize|op
     *      Turns on optimization of scripts.
     *     -output|o=<string>
     *      Sets the compiler's output directory.
     *     -import|i=<string>
     *      Sets the compiler's import directories, separated by semicolons.
     *     -flags|f=<string>
     *      Sets the file to use for user-defined flags.
     *     -all|a
     *      Invokes the compiler against all psc files in the specified directory
     *      (interprets object as the folder).
     *     -quiet|q
     *      Does not report progress or success (only failures).
     *     -noasm
     *      Does not generate an assembly file and does not run the assembler.
     *     -keepasm
     *      Keeps the assembly file after running the assembler.
     *     -asmonly
     *      Generates an assembly file but does not run the assembler.
     *     -?
     *      Prints usage information.
     * */

    [Flags]
    public enum CompilerFlags : ushort {
        //None        = 0x0000,
        Debug       = 0x0001,
        Optimize    = 0x0002,
        All         = 0x0004,
        Quiet       = 0x0008,
        NoASM       = 0x0010,
        KeepASM     = 0x0020,
        ASMOnly     = 0x0040,
    }

    public static class ScriptCompiler {
        private static string compilerPath = @"C:\Games\Steam\steamapps\common\Skyrim\Papyrus Compiler\PapyrusCompiler.exe";

        public static OutputWindowPane Output { get; private set; }

        public static void Initialize(Package package) {
            Output = OutputWindowPaneManager.Instance.CreateWindowPane(new Guid(PapyrusGUID.ScriptCompilerOutputGuidString), "Papyrus Compiler");
        }

        public static void StartCompileThread(string fileOrFolder, string outputFolder, IEnumerable<string> additionalImportFolders = null, CompilerFlags additionalFlags = 0) {
            if (!File.Exists(compilerPath)) {
                return;
            }

            List<string> importFolders = new List<string>();
            importFolders.Add(Path.GetDirectoryName(fileOrFolder));
            importFolders.AddRange(PapyrusEditor.ActiveGame.DefaultSourceFolders);
            if (additionalImportFolders != null) {
                importFolders.AddRange(additionalImportFolders);
            }

            ProcessStartInfo compileProcessInfo = new ProcessStartInfo();

            compileProcessInfo.FileName = compilerPath;
            compileProcessInfo.Arguments = String.Format("\"{0}\" -f=\"{1}\" -i=\"{2}\" -o=\"{3}\"{4}",
                fileOrFolder,
                PapyrusEditor.ActiveGame.CompilerFlags,
                String.Join(";", importFolders.Distinct()),
                outputFolder,
                FormatCompilerFlags(additionalFlags));

            //output.PrintLine(compileProcessInfo.Arguments);

            compileProcessInfo.UseShellExecute = false;
            compileProcessInfo.CreateNoWindow = true;

            compileProcessInfo.RedirectStandardOutput = true;
            compileProcessInfo.RedirectStandardError = true;

            Process compileProcess = new Process();
            compileProcess.StartInfo = compileProcessInfo;

            compileProcess.OutputDataReceived += CompileProcess_OutputDataReceived;
            compileProcess.ErrorDataReceived += CompileProcess_ErrorDataReceived;
            //compileProcess.Exited += CompileProcess_Exited;
            
            compileProcess.EnableRaisingEvents = true;

            compileProcess.Start();
            compileProcess.BeginOutputReadLine();
            compileProcess.BeginErrorReadLine();
        }

        public static void ClearOutput() {
            Output.Clear();
        }

        private static void CompileProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Output.PrintLine(e.Data);
            Output.Show();
        }
        private static void CompileProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Output.PrintLine(e.Data);
            Output.Show();
        }

        /*
        private static void CompileProcess_Exited(object sender, EventArgs e) {
            Process p = sender as Process;
            //MessageBox.Show(String.Format("Compiler exit: {0}", p.ExitCode), "Papyrus Compiler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        */

        private static string FormatCompilerFlags(CompilerFlags flags) {
            StringBuilder b = new StringBuilder();

            foreach (CompilerFlags f in Enum.GetValues(typeof(CompilerFlags))) {
                if (flags.HasFlag(f)) {
                    b.AppendWhiteSpace();
                    b.Append(f);
                }
            }

            return b.ToString();
        }
    }
}
