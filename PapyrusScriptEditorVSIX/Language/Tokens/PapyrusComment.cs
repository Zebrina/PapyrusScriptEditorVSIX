using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace Papyrus.Language.Tokens {
    [DebuggerStepThrough]
    public sealed class PapyrusComment : IPapyrusToken, ISyntaxColorableToken, IOutlineableToken {
        public const char LineBegin = ';';
        public const string BlockBegin = ";/";
        public const string BlockEnd = "/;";

        public PapyrusTokenType Type { get { return PapyrusTokenType.Comment; } }
        public string Text { get; private set; }
        public bool IsBlock { get; private set; }
        public int TokenSize { get { return Text.Length; } }
        public bool IsCompileTimeConstant { get { return true; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusComment(string text, bool isBlock) {
            this.Text = text;
            this.IsBlock = isBlock;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusCommentColorFormat.Name);
        }

        bool IOutlineableToken.IsOutlineableStart(IReadOnlyTokenSnapshotLine line) {
            return IsBlock && Text.StartsWith(BlockBegin) && !Text.EndsWith(BlockEnd);
        }
        bool IOutlineableToken.IsOutlineableEnd(IOutlineableToken startToken) {
            return IsBlock && Text.EndsWith(BlockEnd) && !Text.StartsWith(BlockBegin);
        }
        bool IOutlineableToken.IsImplementation {
            get { return false; }
        }
        string IOutlineableToken.CollapsedText {
            get { return ";/ ... /;"; }
        }
        bool IOutlineableToken.CollapseFirstLine {
            get { return true; }
        }
    }

    internal sealed class PapyrusLineCommentParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                if (context.Source.First() == PapyrusComment.LineBegin) {
                    token = new PapyrusComment(context.Source, false);
                    return true;
                }
            }
            token = null;
            return false;
        }
    }

    internal sealed class PapyrusBlockCommentParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                if (String.Compare(context.Source, 0, PapyrusComment.BlockBegin, 0, PapyrusComment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                    int endOffset = context.Source.IndexOf(PapyrusComment.BlockEnd, PapyrusComment.BlockBegin.Length);
                    if (endOffset == -1) {
                        context.Scanner.GoToState(TokenScannerState.Comment);
                        token = new PapyrusComment(context.Source, true);
                    }
                    else {
                        token = new PapyrusComment(context.Source.Substring(0, endOffset + PapyrusComment.BlockEnd.Length), true);
                    }
                    return true;
                }
            }
            else if (context.Scanner.CurrentState == TokenScannerState.Comment) {
                int endOffset = context.Source.IndexOf(PapyrusComment.BlockEnd);
                if (endOffset == -1) {
                    token = new PapyrusComment(context.Source, true);
                }
                else {
                    context.Scanner.GoToPreviousState();
                    token = new PapyrusComment(context.Source.Substring(0, endOffset + PapyrusComment.BlockEnd.Length), true);
                }
                return true;
            }

            token = null;
            return false;
        }
    }

    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PapyrusCommentColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusComment";

        internal PapyrusCommentColorFormat() {
            DisplayName = "Papyrus Comment";
            ForegroundColor = Color.FromRgb(87, 166, 74);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusCommentColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusCommentColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
