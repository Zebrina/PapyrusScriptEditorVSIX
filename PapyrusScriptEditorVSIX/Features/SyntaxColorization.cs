//------------------------------------------------------------------------------
// <copyright file="SyntaxColorization.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Papyrus.Features {
    public interface ISyntaxColorable {
        IClassificationType GetClassificationType(IClassificationTypeRegistryService registry);
    }

    /// <summary>
    /// Classifier that classifies all text as an instance of the "SyntaxColorization" classification type.
    /// </summary>
    internal class SyntaxColorization : IClassifier {
        private IClassificationTypeRegistryService registry;
        private ITextBuffer buffer;
        private ITextSnapshot snapshot;
        private IReadOnlyTokenSnapshot tokenSnapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxColorization"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal SyntaxColorization(IClassificationTypeRegistryService registry, ITextBuffer buffer) {
            this.registry = registry;
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.buffer.Changed += Buffer_Changed;
            BackgroundParser.Singleton.RequestParse(buffer.CurrentSnapshot);
            this.tokenSnapshot = BackgroundParser.Singleton.TokenSnapshot;
        }

        private void Buffer_Changed(object sender, TextContentChangedEventArgs e) {
            if (e.After == buffer.CurrentSnapshot) {
                ReParse();
            }
        }

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
            var result = new List<ClassificationSpan>();
            foreach (var token in tokenSnapshot.Tokens) {
                ISyntaxColorable colorable = token.Type as ISyntaxColorable;
                if (colorable != null) {
                    result.Add(new ClassificationSpan(token.Span, colorable.GetClassificationType(registry)));
                }
            }

            return result;
        }

        private void ReParse() {
            BackgroundParser.Singleton.RequestParse(buffer.CurrentSnapshot);

            ITextSnapshot newSnapshot = buffer.CurrentSnapshot;
            IReadOnlyTokenSnapshot newTokenSnapshot = BackgroundParser.Singleton.TokenSnapshot;

            List<Span> oldSpans = new List<Span>(tokenSnapshot.Tokens.Select(t => t.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive).Span));
            List<Span> newSpans = new List<Span>(newTokenSnapshot.Tokens.Select(t => t.Span.Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

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
            tokenSnapshot = newTokenSnapshot;

            if (changeStart <= changeEnd) {
                ITextSnapshot snap = snapshot;
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }
    }

    /// <summary>
    /// Classifier provider. It adds the classifier to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(PapyrusContentDefinition.ContentType)]
    internal class SyntaxColorizationProvider : IClassifierProvider {
        /// <summary>
        /// Classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService classificationRegistry = null;

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer) {
            return buffer.Properties.GetOrCreateSingletonProperty(creator: () => new SyntaxColorization(classificationRegistry, buffer));
        }
    }
}
