using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Papyrus.Features {
    internal class IntellisenseController : IIntellisenseController {
        private ITextView m_textView;
        private IList<ITextBuffer> m_subjectBuffers;
        private IntellisenseControllerProvider m_provider;
        private IQuickInfoSession m_session;

        public IntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers, IntellisenseControllerProvider provider) {
            m_textView = textView;
            m_subjectBuffers = subjectBuffers;
            m_provider = provider;

            m_textView.MouseHover += this.OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
            //find the mouse position by mapping down to the subject buffer
            SnapshotPoint? point = m_textView.BufferGraph.MapDownToFirstMatch
                 (new SnapshotPoint(m_textView.TextSnapshot, e.Position),
                PointTrackingMode.Positive,
                snapshot => m_subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);

            if (point != null) {
                ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
                PointTrackingMode.Positive);

                if (!m_provider.QuickInfoBroker.IsQuickInfoActive(m_textView)) {
                    m_session = m_provider.QuickInfoBroker.TriggerQuickInfo(m_textView, triggerPoint, true);
                }
            }
        }

        public void Detach(ITextView textView) {
            if (m_textView == textView) {
                m_textView.MouseHover -= this.OnTextViewMouseHover;
                m_textView = null;
            }
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }
        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }
    }

    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Intellisense Controller")]
    [ContentType(PapyrusContentDefinition.ContentType)]
    internal class IntellisenseControllerProvider : IIntellisenseControllerProvider {
        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers) {
            return new IntellisenseController(textView, subjectBuffers, this);
        }
    }
}
