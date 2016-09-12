using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

/*
namespace Papyrus {
    internal class PapyrusAutoCompleteSource : ICompletionSource {
        private PapyrusAutoCompleteSourceProvider sourceProvider;
        private ITextBuffer textBuffer;
        private List<Completion> compList;

        public PapyrusAutoCompleteSource(PapyrusAutoCompleteSourceProvider sourceProvider, ITextBuffer textBuffer) {
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
    [ContentType(PapyrusLanguageService.ContentType)]
    [Name("token completion")]
    internal class PapyrusAutoCompleteSourceProvider : ICompletionSourceProvider {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer) {
            return new PapyrusAutoCompleteSource(this, textBuffer);
        }
    }
}
*/
