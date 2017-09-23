using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus {
    [Guid(PapyrusGUID.ProjectFactoryGuidString)]
    internal class PapyrusProjectFactory : IVsProjectFactory {
        private const string PapyrusProjectFactoryGuidString = "32FFC1EC-912F-450C-B222-3EB3B6CAA8F5";

        private Package package;
        private uint handle = 0;

        public PapyrusProjectFactory(Package package) {
            this.package = package;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp) {
            IVsRegisterProjectTypes registerProjectTypeService = (IVsRegisterProjectTypes)PackageUtilities.QueryService<IVsRegisterProjectTypes>(psp);
            if (registerProjectTypeService == null) {
                return VSConstants.E_FAIL;
            }

            return registerProjectTypeService.RegisterProjectType(new Guid(PapyrusGUID.ProjectFactoryGuidString), this, out handle);
        }

        public int CanCreateProject(string pszFilename, uint grfCreateFlags, out int pfCanCreate) {
            pfCanCreate = File.Exists(pszFilename) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public int CreateProject(string pszFilename, string pszLocation, string pszName, uint grfCreateFlags, ref Guid iidProject, out IntPtr ppvProject, out int pfCanceled) {
            ppvProject = IntPtr.Zero;
            pfCanceled = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int Close() {
            return VSConstants.E_NOTIMPL;
        }
    }
}
