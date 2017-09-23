using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

// https://msdn.microsoft.com/en-us/library/ee197665.aspx

namespace Papyrus.Features {
    public interface IOutlineableToken {
        bool IsOutlineableStart(IReadOnlyTokenSnapshotLine line);
        bool IsOutlineableEnd(IOutlineableToken startToken);
        bool IsImplementation { get; }
        string CollapsedText { get; }
        bool CollapseFirstLine { get; }
    }

    internal sealed class OutliningTag : IOutliningRegionTag {
        private IOutlineableToken token;
        private string hintText;

        public OutliningTag(IOutlineableToken token, string hintText) {
            this.token = token;
            this.hintText = hintText;
        }

        bool IOutliningRegionTag.IsDefaultCollapsed {
            get { return false; }
        }
        bool IOutliningRegionTag.IsImplementation {
            get { return token.IsImplementation; }
        }

        object IOutliningRegionTag.CollapsedForm {
            get { return token.CollapsedText; }
        }
        object IOutliningRegionTag.CollapsedHintForm {
            get { return hintText; }
        }
    }

    /// <summary>
    /// Outlining for properties, functions, events and states.
    /// https://msdn.microsoft.com/en-us/library/ee197665.aspx
    /// </summary>
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag> {
        private class PartialRegion {
            public IOutlineableToken OutlineableToken { get; set; }
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
            this.buffer.Changed += Buffer_Changed;
            PapyrusEditor.TargetGameInfoChanged += PapyrusEditor_TargetGameInfoChanged;
        }

        void Buffer_Changed(object sender, TextContentChangedEventArgs e) {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After == buffer.CurrentSnapshot) {
                ReParse();
            }
        }

        private void PapyrusEditor_TargetGameInfoChanged(object sender, TargetGameInfoChangedEventArgs e) {
            BackgroundParser.Singleton.ForceReParse(buffer.CurrentSnapshot);
            ReParse();
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (spans.Count == 0) {
                yield break;
            }

            List<Region> currentRegions = regions;
            ITextSnapshot currentSnapshot = snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;
            foreach (var region in currentRegions) {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber) {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(region.OutlineableToken.CollapseFirstLine ? startLine.Start : startLine.End, endLine.End),
                        new OutliningTag(region.OutlineableToken, currentSnapshot.GetText(startLine.Start, endLine.End - startLine.Start)));
                }
            }
        }

        void ReParse() {
            BackgroundParser.Singleton.RequestReParse(buffer.CurrentSnapshot);

            ITextSnapshot newSnapshot = buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();

            // keep the current (deepest) partial region, which will have
            // references to any parent partial regions.
            PartialRegion currentRegion = null;

            foreach (var line in BackgroundParser.Singleton.TokenSnapshot) {
                foreach (var token in line) {
                    IOutlineableToken outlineableToken = token.Type as IOutlineableToken;
                    if (outlineableToken != null) {
                        if (outlineableToken.IsOutlineableStart(line)) {
                            currentRegion = new PartialRegion() {
                                OutlineableToken = outlineableToken,
                                StartLine = newSnapshot.GetLineFromPosition(token.Span.Start.Position).LineNumber,
                                PartialParent = currentRegion,
                            };
                            break;
                        }
                        else if (currentRegion != null && outlineableToken.IsOutlineableEnd(currentRegion.OutlineableToken)) {
                            newRegions.Add(new Region() {
                                OutlineableToken = currentRegion.OutlineableToken,
                                StartLine = currentRegion.StartLine,
                                EndLine = newSnapshot.GetLineFromPosition(token.Span.Start.Position).LineNumber,
                            });

                            currentRegion = currentRegion.PartialParent;
                            break;
                        }
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
            return buffer.Properties.GetOrCreateSingletonProperty(delegate () {
                return new OutliningTagger(buffer) as ITagger<T>;
            });
        }
    }
} 