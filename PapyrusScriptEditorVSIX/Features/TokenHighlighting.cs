using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;

/*
namespace Papyrus.Features {
    public interface IHighlightableToken {
    }

    internal class TokenHighlightingTag : TextMarkerTag {
        public TokenHighlightingTag() :
            base(TokenHighlighting.Name) {
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name(Name)]
    [UserVisible(false)]
    internal class TokenHighlighting : MarkerFormatDefinition {
        internal const string Name = "PapyrusTokenHighlighting";

        public TokenHighlighting() {
            this.ForegroundColor = Color.FromRgb(173, 192, 211);
            this.BackgroundColor = Color.FromRgb(14, 69, 131);
            this.DisplayName = "Papyrus Token Highlighting";
            this.ZOrder = 5;
        }
    }

    internal class TokenHighlightingTagger : ITagger<TokenHighlightingTag> {
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private ITextView view;
        private ITextBuffer sourceBuffer;
        private NormalizedSnapshotSpanCollection wordSpans;
        private SnapshotSpan? currentWord;
        private SnapshotPoint requestedPoint;

        private object updateLock = new object();

        public TokenHighlightingTagger(ITextView view, ITextBuffer sourceBuffer) {
            this.view = view;
            this.sourceBuffer = sourceBuffer;
            this.wordSpans = new NormalizedSnapshotSpanCollection();
            this.currentWord = null;
            this.view.Caret.PositionChanged += CaretPositionChanged;
            this.view.LayoutChanged += ViewLayoutChanged;
        }

        private void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            if (e.NewSnapshot != e.OldSnapshot) {
                BackgroundParser.Singleton.RequestReParse(e.NewSnapshot);
                UpdateAtCaretPosition(view.Caret.Position);
            }
        }

        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            UpdateAtCaretPosition(e.NewPosition);
        }

        private void UpdateAtCaretPosition(CaretPosition caretPosition) {
            SnapshotPoint? point = caretPosition.Point.GetPoint(sourceBuffer, caretPosition.Affinity);

            if (!point.HasValue) {
                return;
            }

            // If the new caret position is still within the current word (and on the same snapshot), we don't need to check it 
            if (currentWord.HasValue && currentWord.Value.Snapshot == view.TextSnapshot
                && point.Value >= currentWord.Value.Start && point.Value <= currentWord.Value.End) {
                return;
            }

            requestedPoint = point.Value;
            UpdateWordAdornments();
        }

        private void UpdateWordAdornments() {
            SnapshotPoint currentRequest = requestedPoint;

            IReadOnlyTokenSnapshot tokenSnapshot = BackgroundParser.Singleton.TokenSnapshot;
            if (tokenSnapshot != null) {
                PapyrusTokenInfo selectedToken = tokenSnapshot.ParseableTokens.SingleOrDefault(t => {
                    return t.Type.TypeID == TokenTypeID.Identifier &&
                        (t.Span.Contains(currentRequest) || t.Span.End == currentRequest);
                });
                NormalizedSnapshotSpanCollection newWordSpans = null;
                if (selectedToken != null) {
                    newWordSpans = new NormalizedSnapshotSpanCollection(tokenSnapshot.ParseableTokens.Where(t => t.TypeEquals(selectedToken)).Select(t => t.Span));
                }
                
                // If another change hasn't happened, do a real update.
                if (currentRequest == requestedPoint) {
                    SynchronousUpdate(currentRequest, newWordSpans, selectedToken != null ? selectedToken.Span : default(SnapshotSpan));
                }
            }
        }
        private static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word) {
            return word.IsSignificant && currentRequest.Snapshot.GetText(word.Span).Any(c => char.IsLetter(c));
        }

        private void SynchronousUpdate(SnapshotPoint currentRequest, NormalizedSnapshotSpanCollection newSpans, SnapshotSpan? newCurrentWord) {
            lock (updateLock) {
                if (currentRequest == requestedPoint) {
                    wordSpans = newSpans;
                    currentWord = newCurrentWord;

                    TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(sourceBuffer.CurrentSnapshot, 0, sourceBuffer.CurrentSnapshot.Length)));
                }
            }
        }

        public IEnumerable<ITagSpan<TokenHighlightingTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (this.currentWord == null || this.wordSpans == null) {
                yield break;
            }

            // Hold on to a "snapshot" of the word spans and current word, so that we maintain the same collection throughout.
            SnapshotSpan currentWord = this.currentWord.Value;
            NormalizedSnapshotSpanCollection wordSpans = this.wordSpans;

            if (spans.Count == 0 || wordSpans.Count == 0) {
                yield break;
            }

            // If the requested snapshot isn't the same as the one our words are on, translate our spans to the expected snapshot 
            if (spans.First().Snapshot != wordSpans.First().Snapshot) {
                wordSpans = new NormalizedSnapshotSpanCollection(wordSpans.Select(span => span.TranslateTo(spans.First().Snapshot, SpanTrackingMode.EdgeExclusive)));
                currentWord = currentWord.TranslateTo(spans.First().Snapshot, SpanTrackingMode.EdgeExclusive);
            }

            // First, yield back the word the cursor is under (if it overlaps) 
            // Note that we'll yield back the same word again in the wordspans collection; 
            // the duplication here is expected. 
            if (spans.OverlapsWith(new NormalizedSnapshotSpanCollection(currentWord)))
                yield return new TagSpan<TokenHighlightingTag>(currentWord, new TokenHighlightingTag());

            // Second, yield all the other words in the file 
            foreach (SnapshotSpan span in NormalizedSnapshotSpanCollection.Overlap(spans, wordSpans)) {
                yield return new TagSpan<TokenHighlightingTag>(span, new TokenHighlightingTag());
            }
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [TagType(typeof(TextMarkerTag))]
    internal class TokenHighlightingTaggerProvider : IViewTaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            // Provide highlighting only on the top buffer 
            if (textView.TextBuffer != buffer) {
                return null;
            }

            return new TokenHighlightingTagger(textView, buffer) as ITagger<T>;
        }
    }
}
*/
