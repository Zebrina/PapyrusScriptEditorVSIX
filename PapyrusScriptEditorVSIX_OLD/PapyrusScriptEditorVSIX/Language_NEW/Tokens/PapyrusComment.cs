using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language_NEW.Parsing;
using Papyrus.Language_NEW.Parsing.Interfaces;
using Papyrus.Language_NEW.Tokens;
using Papyrus.Language_NEW.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Papyrus.Language_NEW.Data;

// READY
namespace Papyrus.Language_NEW.Tokens {
    [DebuggerStepThrough]
    public class PapyrusComment : IPapyrusToken, ISyntaxColorableToken {
        public const char LineBegin = ';';
        public const string BlockBegin = ";/";
        public const string BlockEnd = "/;";

        public PapyrusTokenType Type { get { return PapyrusTokenType.Comment; } }
        public string Text { get; private set; }
        public bool IsBlock { get; private set; }
        public int TokenSize { get { return Text.Length + 2; } }
        public bool IsCompileTimeConstant { get { return true; } }
        public bool IsIgnoredByParser { get { return false; } }
        public bool IsLineExtension { get { return false; } }

        public PapyrusComment(string text, bool isBlock) {
            this.Text = text;
            this.IsBlock = isBlock;
        }

        public bool IsEqualToToken(IPapyrusToken other) {
            throw new NotImplementedException();
        }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            throw new NotImplementedException();
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusCommentColorFormat.Name);
        }
    }

    internal sealed class PapyrusLineCommentParser : ITokenParser {
        public bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, out IPapyrusToken token) {
            if (scanner.CurrentState == TokenScannerState.Default) {
                if (sourceTextSpan.First() == PapyrusComment.LineBegin) {
                    token = new PapyrusComment(sourceTextSpan, false);
                    return true;
                }
            }
            token = null;
            return false;
        }
    }

    internal sealed class PapyrusBlockCommentParser : ITokenParser {
        public bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, out IPapyrusToken token) {
            int endOffset, length;

            if (scanner.CurrentState == TokenScannerState.Default) {
                if (String.Compare(sourceTextSpan, 0, PapyrusComment.BlockBegin, 0, PapyrusComment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                    endOffset = sourceTextSpan.IndexOf(PapyrusComment.BlockEnd, PapyrusComment.BlockBegin.Length);
                    if (endOffset == -1) {
                        scanner.CurrentState = TokenScannerState.Comment;
                        token = new PapyrusComment(sourceTextSpan, true);
                    }
                    else {
                        length = (endOffset + PapyrusComment.BlockEnd.Length);
                        token = new PapyrusComment(sourceTextSpan.Substring(0, length), true);
                    }
                    return true;
                }
            }
            else if (scanner.CurrentState == TokenScannerState.Comment) {
                endOffset = sourceTextSpan.IndexOf(PapyrusComment.BlockEnd);
                if (endOffset == -1) {
                    token = new PapyrusComment(sourceTextSpan, true);
                }
                else {
                    scanner.CurrentState = TokenScannerState.Default;
                    length = (endOffset + PapyrusComment.BlockEnd.Length);
                    token = new PapyrusComment(sourceTextSpan.Substring(0, length), true);
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
