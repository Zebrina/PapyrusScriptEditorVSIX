#if false
using Papyrus.Common;
using Papyrus.Language.Components;
using Papyrus.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Papyrus.Language.Parsing {
    [Flags]
    public enum TokenParserFlags {
        None = 0x0000,
        Comments = 0x0001,
        Strings = 0x0002,
        NumericalLiterals = 0x0004,
        Braces = 0x0008,
        Operators = 0x0010,
        Delimiters = 0x0020,
        Keywords = 0x0040,
        ScriptObjects = 0x0080,
        Identifiers = 0x0100,
        All = 0xFFFF,
    }

    public enum TokenParserResult {
        EndSource,
        EndLine,
        ExtendedLine,
    }

    public enum TokenParserState {
        Text,
        Comment,
        Documentation,
        ParameterList,
    }

    public class TokenParser {
        private TokenTypeScanner tokenScanner;
        private TokenParserState state;
        private OutputWindowPane output;

        public TokenParser(TokenParserFlags flags = TokenParserFlags.All) {
            tokenScanner = new LineExtensionTokenScanner();
            tokenScanner.AddChain(new CommentTokenScanner());
            tokenScanner.AddChain(new StringLiteralTokenScanner());
            tokenScanner.AddChain(new WhiteSpaceTokenScanner());

            if (flags.HasFlag(TokenParserFlags.NumericalLiterals)) {
                tokenScanner.AddChain(new NumericLiteralTokenScanner());
            }
            if (flags.HasFlag(TokenParserFlags.Operators)) {
                tokenScanner.AddChain(new OperatorTokenScanner());
            }
            if (flags.HasFlag(TokenParserFlags.Delimiters)) {
                tokenScanner.AddChain(new DelimiterTokenScanner());
            }
            if (flags.HasFlag(TokenParserFlags.Keywords)) {
                tokenScanner.AddChain(new KeywordTokenScanner());
            }
            if (flags.HasFlag(TokenParserFlags.ScriptObjects)) {
                tokenScanner.AddChain(new ScriptObjectScanner());
            }
            if (flags.HasFlag(TokenParserFlags.Identifiers)) {
                tokenScanner.AddChain(new IdentifierTokenScanner());
            }

            state = TokenParserState.Text;
        }

        public TokenParserState State {
            get { return state; }
            set { state = value; }
        }

        public bool ParseToken(string source, int offset, out TokenType token) {
            try {
                if (offset < source.Length) {
                    token = tokenScanner.ScanForToken(source, offset, ref state);
                    return !TokenType.IsNullOrInvalid(token);
                }
            }
            catch {

            }

            token = null;
            return false;
        }
        public TokenParserResult ParseLine(string line, int lineOffset, ICollection<TokenType> tokens) {


            if (line == null) {
                return TokenParserResult.EndSource;
            }

            TokenType token;
            bool lineEnd = true;

            while (ParseToken(line, lineOffset, out token)) {
                if (!token.IgnoredInLine) {
                    tokens.Add(token);
                }
                if (token.ExtendsLine) {
                    lineEnd = false;
                }

                lineOffset += token.Text.Length;
            }

            return lineEnd ? TokenParserResult.EndLine : TokenParserResult.ExtendedLine;
        }
        public TokenParserResult ParseLine(TextReader reader, ICollection<TokenType> tokens) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                if (ParseLine(line, 0, tokens) == TokenParserResult.EndLine) {
                    return TokenParserResult.EndLine;
                }
            }

            return TokenParserResult.EndSource;
        }

        #region Token scanners

        private abstract class TokenTypeScanner {
            private TokenTypeScanner next = null;

            public abstract TokenType ScanForToken(string source, int offset, ref TokenParserState state);

            //[DebuggerStepThrough]
            protected TokenType YieldToNext(string source, int offset, ref TokenParserState state) {
                return next == null ? null : next.ScanForToken(source, offset, ref state);
            }

            public void AddChain(TokenTypeScanner newTokenScanner) {
                if (next == null) {
                    next = newTokenScanner;
                }
                else {
                    next.AddChain(newTokenScanner);
                }
            }
        }

        private sealed class LineExtensionTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                if (state == TokenParserState.Text) {
                    Delimiter delimiter = Delimiter.Parse(source[offset]);
                    if (delimiter != null && delimiter == Delimiter.Backslash) {
                        return delimiter;
                    }
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class CommentTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                int endOffset;

                switch (state) {
                    case TokenParserState.Text:
                        if (String.Compare(source, offset, Comment.BlockBegin, 0, Comment.BlockBegin.Length, StringComparison.OrdinalIgnoreCase) == 0) {
                            endOffset = source.IndexOf(Comment.BlockEnd, offset + Comment.BlockBegin.Length);
                            if (endOffset == -1) {
                                state = TokenParserState.Comment;
                                return new Comment(source.Substring(offset), true);
                            }
                            return new Comment(source.Substring(offset, (endOffset + Comment.BlockEnd.Length) - offset), true);
                        }
                        else if (source[offset] == (char)Delimiter.SemiColon) {
                            return new Comment(source.Substring(offset), false);
                        }
                        else if (source[offset] == (char)Delimiter.LeftCurlyBracket) {
                            endOffset = source.IndexOf((char)Delimiter.RightCurlyBracket, offset + 1);
                            if (endOffset == -1) {
                                state = TokenParserState.Documentation;
                                return new CreationKitDocumentation(source.Substring(offset));
                            }
                            return new CreationKitDocumentation(source.Substring(offset, (endOffset + 1) - offset));
                        }

                        break;

                    case TokenParserState.Comment:
                        endOffset = source.IndexOf(Comment.BlockEnd, offset);
                        if (endOffset == -1) {
                            return new Comment(source.Substring(offset), true);
                        }
                        state = TokenParserState.Text;
                        return new Comment(source.Substring(offset, (endOffset + Comment.BlockEnd.Length) - offset), true);

                    case TokenParserState.Documentation:
                        endOffset = source.IndexOf((char)Delimiter.RightCurlyBracket, offset);
                        if (endOffset == -1) {
                            return new CreationKitDocumentation(source.Substring(offset));
                        }
                        state = TokenParserState.Text;
                        return new CreationKitDocumentation(source.Substring(offset, (endOffset + 1) - offset));
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class StringLiteralTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                if (source[offset] == (char)Delimiter.QuotationMark) {
                    int endQuote;
                    if ((endQuote = Delimiter.FindNext(source, offset + 1, (char)Delimiter.QuotationMark)) < source.Length) {
                        return new StringLiteral(source.Substring(offset + 1, endQuote - (offset + 1)));
                    }
                }
                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class WhiteSpaceTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                string whiteSpace = new string(source.Substring(offset).TakeWhile(c => Char.IsWhiteSpace(c)).ToArray());
                if (whiteSpace.Length > 0) {
                    return new WhiteSpace(whiteSpace.Length);
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class NumericLiteralTokenScanner : TokenTypeScanner {
            private const string NumberFormatCharacters = "-0123456789XxAaBbCcDdEeFf.";

            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                int nextDelimiter = Delimiter.FindNext(source, offset);
                NumericLiteral numericLiteral = NumericLiteral.Parse(source.Substring(offset, nextDelimiter - offset));
                if (numericLiteral != null) {
                    return numericLiteral;
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class OperatorTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                Operator op = Operator.Parse(source, offset);
                if (op != null) {
                    return op;
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class DelimiterTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                Delimiter delimiter = Delimiter.Parse(source[offset]);
                if (delimiter != null) {
                    return delimiter;
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class KeywordTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                Keyword keyword = Keyword.Parse(source, offset, Delimiter.FindNext(source, offset) - offset);
                if (keyword != null) {
                    return keyword;
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class ScriptObjectScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                int nextDelimiter = Delimiter.FindNext(source, offset);
                if (nextDelimiter > offset) {
                    ScriptObject scriptObject;
                    if (ScriptObject.TryParse(source.Substring(offset, nextDelimiter - offset), out scriptObject)) {
                        return scriptObject;
                    }
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        private sealed class IdentifierTokenScanner : TokenTypeScanner {
            public override TokenType ScanForToken(string source, int offset, ref TokenParserState state) {
                int length = Delimiter.FindNext(source, offset) - offset;

                if (Identifier.IsValid(source.Substring(offset, length))) {
                    return new Identifier(source.Substring(offset, length));
                }

                return YieldToNext(source, offset, ref state);
            }
        }

        #endregion
    }
}

#endif