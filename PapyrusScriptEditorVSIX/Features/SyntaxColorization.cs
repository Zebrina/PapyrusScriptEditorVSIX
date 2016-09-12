//------------------------------------------------------------------------------
// <copyright file="SyntaxColorization.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language;
using Papyrus.Language.Components;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Papyrus.Features {
    internal interface ISyntaxColorable {
        IClassificationType GetClassificationType(IClassificationTypeRegistryService registry);
    }

    /// <summary>
    /// Classifier that classifies all text as an instance of the "SyntaxColorization" classification type.
    /// </summary>
    internal class SyntaxColorization : IClassifier {
        private readonly IClassificationTypeRegistryService registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxColorization"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal SyntaxColorization(IClassificationTypeRegistryService registry) {
            this.registry = registry;
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
            TokenScannerModule scanner = new BlockCommentTokenScanner();

            scanner += new LineCommentTokenScanner();
            scanner += new StringLiteralTokenScanner();
            scanner += new NumericLiteralTokenScanner();
            scanner += new OperatorTokenScanner();
            scanner += new KeywordTokenScanner();
            scanner += new ScriptObjectTokenScanner();
            scanner += new IdentifierTokenScanner();

            ParsedLine parsedLine = new ParsedLine();

            List<Token> tokens = new List<Token>();
            TokenScannerState state = TokenScannerState.Text;

            //scanner.Scan(span, 0, ref state, tokens);

            var result = new List<ClassificationSpan>();
            foreach (Token token in tokens) {
                ISyntaxColorable colorable = token.Type as ISyntaxColorable;
                if (colorable != null) {
                    result.Add(new ClassificationSpan(token.Span, colorable.GetClassificationType(registry)));
                }
            }

            return result;
        }

        private void ReParse() {
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
            return buffer.Properties.GetOrCreateSingletonProperty(creator: () => new SyntaxColorization(this.classificationRegistry));
        }
    }
}
