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
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;

/*
namespace Papyrus.Features {
    internal class StatementCompletionSource : ICompletionSource {
        private StatementCompletionSourceProvider sourceProvider;
        private ITextBuffer textBuffer;
        private List<Completion> compList;

        public StatementCompletionSource(StatementCompletionSourceProvider sourceProvider, ITextBuffer textBuffer) {
            this.sourceProvider = sourceProvider;
            this.textBuffer = textBuffer;
        }

        private PapyrusTokenInfo FindTokenInfoAtPosition(ICompletionSession session) {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            PapyrusTokenInfo token = BackgroundParser.Singleton.TokenSnapshot.ParseableTokens.SingleOrDefault(t => t.Type.TypeID == PapyrusTokenType.Identifier && t.Span.Contains(currentPoint));
            return token;
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session) {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = sourceProvider.NavigatorService.GetTextStructureNavigator(textBuffer);
            TextExtent extent = navigator.GetExtentOfWord(currentPoint);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets) {
            BackgroundParser.Singleton.RequestReParse(session.TextView.TextSnapshot);
            var tokens = BackgroundParser.Singleton.TokenSnapshot;

            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;

            int lineNumber, linePosition;
            if (tokens.IndexOfToken(t => t.Type.TypeID == TokenTypeID.Identifier && t.Span.Contains(currentPoint), out lineNumber, out linePosition)) {
                PapyrusTokenInfo tokenInfo = tokens[lineNumber, linePosition];

                if (linePosition > 0 && tokens[lineNumber, linePosition - 1].Type == Delimiter.FullStop) {

                }
                else if (linePosition == 0 || tokens[lineNumber, linePosition - 1].Type != Delimiter.FullStop) {
                    string tokenStr = tokenInfo.Type.Text.ToLower();
                    var keywords = Keyword.Manager.Where(t => {
                        return t.Text.ToLower().Contains(tokenStr) && String.Equals(t.Text, tokenStr, StringComparison.OrdinalIgnoreCase) == false;
                    });
                    var scriptObjects = ScriptObject.Manager.Values.Where(t => {
                        return t.Text.ToLower().Contains(tokenStr) && String.Equals(t.Text, tokenStr, StringComparison.OrdinalIgnoreCase) == false;
                    });

                    var completions = keywords.Select(k => {
                        return new Completion(k.Text, k.Text + " ", "Keyword", null, null);
                    }).Union(scriptObjects.Select(s => {
                        return new Completion(s.Text, s.Text + " ", "ScriptObject", null, null);
                    }));

                    completionSets.Add(new CompletionSet(
                    "Tokens",
                    "Tokens",
                    tokenInfo.Span.Snapshot.CreateTrackingSpan(tokenInfo.Span, SpanTrackingMode.EdgeInclusive),
                    completions,
                    null));
                }
            }
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
    [Name("token completion")]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [Order(Before = "default")]
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

    [ProvideLanguageCodeExpansion(PapyrusGUID.LanguageServiceGuidString, "Papyrus", 0,
        "Papyrus", //the language ID used in the .snippet files
        @"%ProjItem%\Snippets\%LCID%\PapyrusSnippets.xml", //the path of the index file
        SearchPaths = @"%ProjItem%\Snippets\%LCID%\",
        ForceCreateDirs = @"%ProjItem%\Snippets\%LCID%\")]
    internal class StatementCompletionCommandHandler : IOleCommandTarget {
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;
        private StatementCompletionCommandHandlerProvider provider;
        private ICompletionSession session;

        internal StatementCompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, StatementCompletionCommandHandlerProvider provider) {
            this.textView = textView;
            this.provider = provider;

            //add the command to the command chain
            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }

        private void OnSessionDismissed(object sender, EventArgs e) {
            session.Dismissed -= this.OnSessionDismissed;
            session = null;
        }

        private bool TriggerCompletion() {
            //the caret must be in a non-projection location 
            SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(
            textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue) {
                return false;
            }

            session = provider.CompletionBroker.CreateCompletionSession
         (textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            //subscribe to the Dismissed event on the session 
            session.Dismissed += this.OnSessionDismissed;
            session.Start();

            return true;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider)) {
                return nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
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
                if (session != null && !session.IsDismissed) {
                    //if the selection is fully selected, commit the current session
                    if (session.SelectedCompletionSet.SelectionStatus.IsSelected) {
                        session.Commit();
                        //also, don't add the character to the buffer
                        return VSConstants.S_OK;
                    }
                    else {
                        //if there is no selection, dismiss the session
                        session.Dismiss();
                    }
                }
            }

            //pass along the command so the char is added to the buffer
            int retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            bool handled = false;
            if (!typedChar.Equals(char.MinValue) && char.IsLetterOrDigit(typedChar)) {
                if (session == null || session.IsDismissed) {// If there is no active session, bring up completion
                    this.TriggerCompletion();
                }
                else {   //the completion session is already active, so just filter
                    session.Filter();
                }
                handled = true;
            }
            else if (commandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE   //redo the filter if there is a deletion
                || commandID == (uint)VSConstants.VSStd2KCmdID.DELETE) {
                if (session != null && !session.IsDismissed)
                    session.Filter();
                handled = true;
            }
            if (handled) return VSConstants.S_OK;
            return retVal;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            return nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
*/