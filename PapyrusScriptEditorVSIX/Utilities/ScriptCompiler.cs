using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

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
        None        = 0x0000,
        Debug       = 0x0001,
        Optimize    = 0x0002,
        All         = 0x0004,
        Quiet       = 0x0008,
        NoASM       = 0x0010,
        KeepASM     = 0x0020,
        ASMOnly     = 0x0040,
    }

    public static class ScriptCompiler {
        public const string OutputGuidString = "65B48410-04CA-4271-BFD9-5DE841EC15FC";

        private static string compilerPath = @"C:\Games\Steam\steamapps\common\Skyrim\Papyrus Compiler\PapyrusCompiler.exe";

        private static OutputWindowPane output;

        public static void Initialize(Package package) {
            output = OutputWindowPaneManager.Instance.CreateWindowPane(new Guid(OutputGuidString), "Papyrus Compiler");

            output.Print("HEJ!");
        }

        public static void StartCompileThread(string fileOrFolder, string outputFolder, string[] importFolders, string flags, CompilerFlags argumentFlags = CompilerFlags.None) {
            if (!File.Exists(compilerPath)) {
                return;
            }

            ProcessStartInfo compileProcessInfo = new ProcessStartInfo();

            compileProcessInfo.FileName = compilerPath;
            compileProcessInfo.Arguments = String.Format("\"{0}\" -f=\"{1}\" i=\"{2}\" -o=\"{3}\"{4}",
                fileOrFolder,
                flags,
                String.Join(";", importFolders),
                outputFolder,
                FormatCompilerFlags(argumentFlags));

            compileProcessInfo.UseShellExecute = true;
            compileProcessInfo.CreateNoWindow = true;

            compileProcessInfo.RedirectStandardOutput = true;
            compileProcessInfo.RedirectStandardError = true;

            Process compileProcess = Process.Start(compileProcessInfo);

            compileProcess.OutputDataReceived += CompileProcess_OutputDataReceived;
            compileProcess.ErrorDataReceived += CompileProcess_ErrorDataReceived;
            compileProcess.Exited += CompileProcess_Exited;

            compileProcess.EnableRaisingEvents = true;
        }

        private static void CompileProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            output.Print(e.Data);
            output.Show();
        }

        private static void CompileProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            output.Print(e.Data);
            output.Show();
        }

        private static void CompileProcess_Exited(object sender, EventArgs e) {
            MessageBox.Show("Compiler exit", (sender is Process).ToString(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private static string FormatCompilerFlags(CompilerFlags flags) {
            StringBuilder b = new StringBuilder();

            foreach (CompilerFlags f in Enum.GetValues(typeof(CompilerFlags))) {
                if (f != CompilerFlags.None && flags.HasFlag(f)) {
                    b.AppendFormat(" {0}", f);
                }
            }

            return b.ToString();
        }
    }
}
