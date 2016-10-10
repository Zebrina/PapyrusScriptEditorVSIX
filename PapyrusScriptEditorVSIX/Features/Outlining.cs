using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

// https://msdn.microsoft.com/en-us/library/ee197665.aspx

namespace Papyrus.Features {
    /// <summary>
    /// Outlining for properties, functions, events and states.
    /// https://msdn.microsoft.com/en-us/library/ee197665.aspx
    /// </summary>
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag> {
        private class PartialRegion {
            public Token Outlineable { get; set; }
            public int StartLine { get; set; }
            public PartialRegion PartialParent { get; set; }
        }

        private class Region : PartialRegion {
            public SnapshotSpan Description { get; set; }
            public int EndLine { get; set; }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private ITextBuffer buffer;
        private ITextSnapshot snapshot;
        private List<Region> regions;

        //[DebuggerStepThrough]
        public OutliningTagger(ITextBuffer buffer) {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.regions = new List<Region>();
            ReParse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (spans.Count == 0) {
                yield break;
            }
            List<Region> currentRegions = this.regions;
            ITextSnapshot currentSnapshot = this.snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;
            foreach (var region in currentRegions) {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber) {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.End, endLine.End),
                        new OutliningRegionTag(false, true, "...", (new SnapshotSpan(startLine.Start, endLine.End)).GetText()));
                }
            }
        }

        void ReParse() {
            BackgroundParser.Singleton.RequestParse(buffer.CurrentSnapshot);

            ITextSnapshot newSnapshot = buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();

            // keep the current (deepest) partial region, which will have
            // references to any parent partial regions.
            PartialRegion currentRegion = null;

            foreach (var line in BackgroundParser.Singleton.TokenSnapshot) {
                foreach (var token in line) {
                    if (token.Type.IsOutlineableStart(line)) {
                        currentRegion = new PartialRegion() {
                            Outlineable = token.Type,
                            StartLine = newSnapshot.GetLineFromPosition(token.Span.Start.Position).LineNumber,
                            PartialParent = currentRegion,
                        };
                        break;
                    }
                    else if (currentRegion != null && token.Type.IsOutlineableEnd(currentRegion.Outlineable)) {
                        newRegions.Add(new Region() {
                            Outlineable = currentRegion.Outlineable,
                            StartLine = currentRegion.StartLine,
                            EndLine = newSnapshot.GetLineFromPosition(token.Span.Start.Position).LineNumber,
                        });

                        currentRegion = currentRegion.PartialParent;
                        break;
                    }
                }
            }

            // determine the changed span, and send a changed event with the new spans
            List<Span> oldSpans = new List<Span>(regions.Select(r => AsSnapshotSpan(r, snapshot).TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive).Span));
            List<Span> newSpans = new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

            // the changed regions are regions that appear in one set or the other, but not both.
            NormalizedSpanCollection removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            int changeStart = Int32.MaxValue;
            int changeEnd = -1;

            if (removed.Count > 0) {
                changeStart = removed.First().Start;
                changeEnd = removed.Last().End;
            }

            if (newSpans.Count > 0) {
                changeStart = Math.Min(changeStart, newSpans.First().Start);
                changeEnd = Math.Max(changeEnd, newSpans.Last().Start);
            }

            snapshot = newSnapshot;
            regions = newRegions;

            if (changeStart <= changeEnd) {
                ITextSnapshot snap = snapshot;
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        void BufferChanged(object sender, TextContentChangedEventArgs e) {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After == buffer.CurrentSnapshot) {
                ReParse();
            }
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot) {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ? startLine : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.End, endLine.End);
        }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType(PapyrusContentDefinition.ContentType)]
    [DebuggerStepThrough]
    internal sealed class OutliningTaggerProvider : ITaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            //create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate () {
                return new OutliningTagger(buffer) as ITagger<T>;
            };
            return buffer.Properties.GetOrCreateSingletonProperty(sc);
        }
    }
} 