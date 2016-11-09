//------------------------------------------------------------------------------
// <copyright file="CompileScriptCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Papyrus.Utilities;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;

namespace Papyrus.Commands {
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CompileScriptCommand {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileScriptCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CompileScriptCommand(Package package) {
            if (package == null) {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null) {
                var menuCommandID = new CommandID(new Guid(PapyrusGUID.CommandSetGuidString), CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CompileScriptCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package) {
            Instance = new CompileScriptCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e) {
            /*
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "CompileScriptCommand";
            */

            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            if (dte != null) {
                ScriptCompiler.ClearOutput();

                if (dte.ActiveDocument == null) {
                    ScriptCompiler.Output.PrintError("No active document.");
                    return;
                }
                if (Path.GetExtension(dte.ActiveDocument.Name) != PapyrusContentDefinition.FileExtension) {
                    ScriptCompiler.Output.PrintError("Active document is not a papyrus script file or does not have the proper extension (*{0}).", PapyrusContentDefinition.FileExtension);
                    return;
                }
                
                ScriptCompiler.StartCompileThread(dte.ActiveDocument.FullName, "C:\\PapyrusScripts");
            }
        }
    }
}
