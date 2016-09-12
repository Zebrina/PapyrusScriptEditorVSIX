using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Utilities {
    /*
     * Usage:
     * PapyrusAssembler object [-D] [-V] [-Q] [-A] [-S] [-?]
     * 
     *   object  Specifies the object to be assembled or disassembled. Assembly looks
     *           for a ".pas" extension. Disassembly looks for a ".pex" extension.
     *   -D      Disassembles the object, instead of assembling it.
     *   -V      Turns on verbose mode.
     *   -Q      Turns on quiet mode. (No status messages, only errors)
     *   -A      Do not assemble/disassemble the file, just load and analyze.
     *   -S      Strips debugging info from a compiled file. Cannot be used with -A
     *   or -D
     *   -?      Prints this usage information
     * */

    [Flags]
    public enum AssemblerFlags : ushort {
        None = 0x0000,
        Disassemble = 0x0001,
        Verbose = 0x0002,
        Quiet = 0x0004,
        Analyze = 0x0010,
        Strip = 0x0020,
    }

    public static class ScriptAssembler {
        public static readonly Guid OutputWindowGuid = new Guid("DE284B5B-0BB5-4D69-9C8D-545F677E2B6A");

    }
}
