using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

/*
namespace Papyrus.Features {
    internal class BraceMatchingTag : TextMarkerTag {
        public BraceMatchingTag() :
            base(BraceMatching.Name) {
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name(Name)]
    [UserVisible(false)]
    internal class BraceMatching : MarkerFormatDefinition {
        internal const string Name = "PapyrusBraceMatching";

        public BraceMatching() {
            //this.ForegroundColor = Color.FromRgb(220, 220, 220);
            this.BackgroundColor = Color.FromRgb(14, 69, 131);
            this.DisplayName = "Papyrus Brace Matching";
            this.ZOrder = 5;
        }
    }

    internal class BraceMatchingTagger : ITagger<BraceMatchingTag> {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private ITextView View { get; set; }
        private ITextBuffer SourceBuffer { get; set; }
        private SnapshotPoint? CurrentChar { get; set; }

        internal BraceMatchingTagger(ITextView view, ITextBuffer sourceBuffer) {
            this.View = view;
            this.SourceBuffer = sourceBuffer;
            this.CurrentChar = null;
            this.View.Caret.PositionChanged += CaretPositionChanged;
            BackgroundParser.Singleton.RequestReParse(view.TextSnapshot);
            this.View.LayoutChanged += ViewLayoutChanged;
        }

        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            if (e.NewSnapshot != e.OldSnapshot) {
                BackgroundParser.Singleton.RequestReParse(e.NewSnapshot);
                UpdateAtCaretPosition(View.Caret.Position);
            }
        }

        void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            UpdateAtCaretPosition(e.NewPosition);
        }
        void UpdateAtCaretPosition(CaretPosition caretPosition) {
            CurrentChar = caretPosition.Point.GetPoint(SourceBuffer, caretPosition.Affinity);

            if (CurrentChar.HasValue) {
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0, SourceBuffer.CurrentSnapshot.Length)));
            }
        }

        public IEnumerable<ITagSpan<BraceMatchingTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (spans.Count == 0) { //there is no content in the buffer
                yield break;
            }

            //don't do anything if the current SnapshotPoint is not initialized or at the end of the buffer
            if (!CurrentChar.HasValue || CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length) {
                yield break;
            }

            //hold on to a snapshot of the current character
            SnapshotPoint currentChar = CurrentChar.Value;

            //if the requested snapshot isn't the same as the one the brace is on, translate our spans to the expected snapshot
            if (spans.First().Snapshot != currentChar.Snapshot) {
                currentChar = currentChar.TranslateTo(spans.First().Snapshot, PointTrackingMode.Positive);
            }

            SnapshotPoint previousChar = currentChar == 0 ? currentChar : currentChar - 1;

            IReadOnlyTokenSnapshot parsedSnapshot = BackgroundParser.Singleton.TokenSnapshot;
            if (parsedSnapshot == null) {
                yield break;
            }

            IReadOnlyTokenSnapshotLine parsedLine;

            try {
                parsedLine = parsedSnapshot.Single(l => l.Any(t => {
                    return t.Span.Contains(currentChar) || (previousChar != currentChar && t.Span.Contains(previousChar));
                }));
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (InvalidOperationException e) {
#pragma warning restore CS0168 // Variable is declared but never used
                yield break;
            }

            PapyrusTokenInfo openingToken = parsedLine.SingleOrDefault(t => t.Span.Contains(currentChar));
            PapyrusTokenInfo closingToken = parsedLine.SingleOrDefault(t => t.Span.Contains(previousChar));

            if (openingToken != null && openingToken.Type.IsOpeningBracer) {
                SnapshotSpan matchedSpan;
                if (FindMatchingSpan(parsedLine, openingToken, out matchedSpan)) {
                    yield return new TagSpan<BraceMatchingTag>(openingToken.Span, new BraceMatchingTag());
                    yield return new TagSpan<BraceMatchingTag>(matchedSpan, new BraceMatchingTag());
                }
            }
            else if (closingToken != null && closingToken.Type.IsClosingBracer) {
                SnapshotSpan matchedSpan;
                if (FindMatchingSpan(parsedLine.Reverse(), closingToken, out matchedSpan)) {
                    yield return new TagSpan<BraceMatchingTag>(matchedSpan, new BraceMatchingTag());
                    yield return new TagSpan<BraceMatchingTag>(closingToken.Span, new BraceMatchingTag());
                }
            }
        }

        private static bool FindMatchingSpan(IEnumerable<PapyrusTokenInfo> line, PapyrusTokenInfo matchWith, out SnapshotSpan matchedSpan) {
            int stackedCount = 0;
            foreach (PapyrusTokenInfo tokenInfo in line.SkipWhile(t => !ReferenceEquals(t, matchWith))) {
                if (tokenInfo.Type == matchWith.Type) {
                    // First token is the bracer we are trying to match so (stackedCount >= 1) always apply.
                    ++stackedCount;
                }
                else if (tokenInfo.Type.MatchesWithBracer(matchWith.Type)) {
                    if (--stackedCount == 0) {
                        matchedSpan = tokenInfo.Span;
                        return true;
                    }
                }
            }

            matchedSpan = default(SnapshotSpan);
            return false;
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [TagType(typeof(TextMarkerTag))]
    internal class BraceMatchingTaggerProvider : IViewTaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (textView == null) {
                return null;
            }

            //provide highlighting only on the top-level buffer
            if (textView.TextBuffer != buffer) {
                return null;
            }

            return new BraceMatchingTagger(textView, buffer) as ITagger<T>;
        }
    }
}
*/