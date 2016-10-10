using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace Papyrus {
    internal class StatementCompletionSource : ICompletionSource {
        private StatementCompletionSourceProvider sourceProvider;
        private ITextBuffer textBuffer;
        private List<Completion> compList;

        public StatementCompletionSource(StatementCompletionSourceProvider sourceProvider, ITextBuffer textBuffer) {
            this.sourceProvider = sourceProvider;
            this.textBuffer = textBuffer;
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session) {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = sourceProvider.NavigatorService.GetTextStructureNavigator(textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
            List<string> strList = new List<string>();
            strList.Add("Open");
            strList.Add("Close");
            compList = new List<Completion>();
            foreach (string str in strList)
                compList.Add(new Completion(str + "(int i)", str, str, null, null));

            completionSets.Add(new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(textBuffer),
                    session),
                compList,
                null));
        }

        private bool isDisposed = false;
        public void Dispose() {
            if (!isDisposed) {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
    }

    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [Name("token completion")]
    internal class StatementCompletionSourceProvider : ICompletionSourceProvider {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
            return new StatementCompletionSource(this, textBuffer);
        }
    }
    [Export(typeof(IVsTextViewCreationListener))]
    [Name("token completion handler")]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class StatementCompletionCommandHandlerProvider : IVsTextViewCreationListener {
        [Import]
        internal IVsEditorAdaptersFactoryService AdapterService = null;
        [Import]
        internal ICompletionBroker CompletionBroker { get; set; }
        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter) {
            ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            Func<StatementCompletionCommandHandler> createCommandHandler = delegate () { return new StatementCompletionCommandHandler(textViewAdapter, textView, this); };
            textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
        }
    }

    internal class StatementCompletionCommandHandler : IOleCommandTarget {
        private IOleCommandTarget m_nextCommandHandler;
        private ITextView m_textView;
        private StatementCompletionCommandHandlerProvider m_provider;
        private ICompletionSession m_session;

        internal StatementCompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, StatementCompletionCommandHandlerProvider provider) {
            this.m_textView = textView;
            this.m_provider = provider;

            //add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
        }

        private void OnSessionDismissed(object sender, EventArgs e) {
            m_session.Dismissed -= this.OnSessionDismissed;
            m_session = null;
        }

        private bool TriggerCompletion() {
            //the caret must be in a non-projection location 
            SnapshotPoint? caretPoint = m_textView.Caret.Position.Point.GetPoint(
            textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue) {
                return false;
            }

            m_session = m_provider.CompletionBroker.CreateCompletionSession
         (m_textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            //subscribe to the Dismissed event on the session 
            m_session.Dismissed += this.OnSessionDismissed;
            m_session.Start();

            return true;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (VsShellUtilities.IsInAutomationFunction(m_provider.ServiceProvider)) {
                return m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            //make a copy of this so we can look at it after forwarding some commands
            uint commandID = nCmdID;
            char typedChar = char.MinValue;
            //make sure the input is a char before getting it
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR) {
                typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }

            //check for a commit character
            if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
                || nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB
                || (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar))) {
                //check for a a selection
                if (m_session != null && !m_session.IsDismissed) {
                    //if the selection is fully selected, commit the current session
                    if (m_session.SelectedCompletionSet.SelectionStatus.IsSelected) {
                        m_session.Commit();
                        //also, don't add the character to the buffer
                        return VSConstants.S_OK;
                    }
                    else {
                        //if there is no selection, dismiss the session
                        m_session.Dismiss();
                    }
                }
            }

            //pass along the command so the char is added to the buffer
            int retVal = m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            bool handled = false;
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar)) {
                if (m_session == null || m_session.IsDismissed) // If there is no active session, bring up completion
                {
                    this.TriggerCompletion();
                    m_session.Filter();
                }
                else    //the completion session is already active, so just filter
                {
                    m_session.Filter();
                }
                handled = true;
            }
            else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE   //redo the filter if there is a deletion
                || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE) {
                if (m_session != null && !m_session.IsDismissed)
                    m_session.Filter();
                handled = true;
            }
            if (handled) return VSConstants.S_OK;
            return retVal;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
