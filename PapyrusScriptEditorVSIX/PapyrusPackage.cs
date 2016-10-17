//------------------------------------------------------------------------------
// <copyright file="PapyrusPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Papyrus.Utilities;
using Papyrus.Commands;

namespace Papyrus {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [DebuggerStepThrough]
    public sealed class PapyrusPackage : Package {
        /// <summary>
        /// PapyrusPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "A4A96C7E-FC50-471C-9626-CE80E0DAC70F";

        //uint componentID;

        /// <summary>
        /// Initializes a new instance of the <see cref="PapyrusPackage"/> class.
        /// </summary>
        public PapyrusPackage() {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            /*
            NativeMethods.AllocConsole();
            var handle = NativeMethods.GetConsoleWindow();
            NativeMethods.ShowWindow(handle, NativeMethods.SW_SHOW);
            Console.WriteLine("YES!");
            */
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();

            /*
            // Proffer the service.
            IServiceContainer serviceContainer = this as IServiceContainer;
            LanguageService service = new PapyrusLanguageService();
            service.SetSite(this);
            serviceContainer.AddService(typeof(PapyrusLanguageService), service, true);

            // Register a timer to call our language service during
            // idle periods.
            IOleComponentManager mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
            if (componentID == 0 && mgr != null) {
                OLECRINFO[] crinfo = new OLECRINFO[1];
                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                              (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                              (uint)_OLECADVF.olecadvfRedrawOff |
                                              (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 1000;
                int hr = mgr.FRegisterComponent(this, crinfo, out componentID);
            }
            */

            OutputWindowPaneManager.Instance.Initialize(this);

            // Initialize library.
            //ScriptLibrary.Instance.Initialize(this);

            // Initialize compiler and assembler.
            ScriptCompiler.Initialize(this);

            // Initialize commands.
            CompileScriptCommand.Initialize(this);
        }

        #endregion
    }
}
