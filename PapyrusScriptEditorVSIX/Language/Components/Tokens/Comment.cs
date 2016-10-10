using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace Papyrus.Language.Components.Tokens {
    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class CommentColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusComment";

        internal CommentColorFormat() {
            DisplayName = "Papyrus Comment";
            ForegroundColor = Color.FromRgb(87, 166, 74);
        }
    }

    [DebuggerStepThrough]
    internal static class CommentColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(CommentColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public sealed class Comment : Token, ISyntaxColorable {
        public const string BlockBegin = ";/";
        public const string BlockEnd = "/;";

        private string text;
        private bool isBlock;

        public Comment(string text, bool isBlock) {
            this.text = text;
            this.isBlock = isBlock;
        }

        public override string Text {
            get { return text; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Comment; }
        }

        public override bool IgnoredInSyntax {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(CommentColorFormat.Name);
        }
    }

    internal sealed class LineCommentParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                if (text.First() == (char)Delimiter.SemiColon) {
                    token.Type = new Comment(text, false);
                    token.Span = sourceSnapshotSpan;
                    return true;
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                if (sourceTextSpan.First() == (char)Delimiter.SemiColon) {
                    token = new Comment(sourceTextSpan, false);
                    return true;
                }
            }
            token = null;
            return false;
        }
    }

    internal sealed class BlockCommentParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            string text = sourceSnapshotSpan.GetText();
            int endOffset, length;
            if (state == TokenScannerState.Text) {
                if (String.Compare(text, 0, Comment.BlockBegin, 0, Comment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                    endOffset = text.IndexOf(Comment.BlockEnd, Comment.BlockBegin.Length);
                    if (endOffset == -1) {
                        state = TokenScannerState.BlockComment;
                        token.Type = new Comment(text, true);
                        token.Span = sourceSnapshotSpan;
                    }
                    else {
                        length = (endOffset + Comment.BlockEnd.Length);
                        token.Type = new Comment(text.Substring(0, length), true);
                        token.Span = sourceSnapshotSpan.Subspan(0, length);
                    }
                    return true;
                }
            }
            else if (state == TokenScannerState.BlockComment) {
                endOffset = text.IndexOf(Comment.BlockEnd);
                if (endOffset == -1) {
                    token.Type = new Comment(text, true);
                    token.Span = sourceSnapshotSpan;
                }
                else {
                    state = TokenScannerState.Text;
                    length = (endOffset + Comment.BlockEnd.Length);
                    token.Type = new Comment(text.Substring(0, length), true);
                    token.Span = sourceSnapshotSpan.Subspan(0, length);
                }
                return true;
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            int endOffset, length;
            if (state == TokenScannerState.Text) {
                if (String.Compare(sourceTextSpan, 0, Comment.BlockBegin, 0, Comment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                    endOffset = sourceTextSpan.IndexOf(Comment.BlockEnd, Comment.BlockBegin.Length);
                    if (endOffset == -1) {
                        state = TokenScannerState.BlockComment;
                        token = new Comment(sourceTextSpan, true);
                    }
                    else {
                        length = (endOffset + Comment.BlockEnd.Length);
                        token = new Comment(sourceTextSpan.Substring(0, length), true);
                    }
                    return true;
                }
            }
            else if (state == TokenScannerState.BlockComment) {
                endOffset = sourceTextSpan.IndexOf(Comment.BlockEnd);
                if (endOffset == -1) {
                    token = new Comment(sourceTextSpan, true);
                }
                else {
                    state = TokenScannerState.Text;
                    length = (endOffset + Comment.BlockEnd.Length);
                    token = new Comment(sourceTextSpan.Substring(0, length), true);
                }
                return true;
            }

            token = null;
            return false;
        }
    }
}
