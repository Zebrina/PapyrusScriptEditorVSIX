using System.Collections.Generic;
using System.Linq;
using Papyrus.Common;
using Papyrus.Language.Parsing;
using Papyrus.Common.Extensions;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusDelimiter : IPapyrusToken {
        private enum MatchingBracerType : byte {
            None,
            OpeningBracer,
            ClosingBracer,
        }

        private struct Data {
            public char Character;
            public char MatchingCharacter;
            public MatchingBracerType MatchingBracerType;
            public bool ExtendsLine;
            public bool IgnoredBySyntax;

            public override int GetHashCode() {
                return Hash.GetMemberwiseHashCode(Character, MatchingCharacter, MatchingBracerType, ExtendsLine, IgnoredBySyntax);
            }
            public override bool Equals(object obj) {
                return obj is Data &&
                    this.Character == ((Data)obj).Character &&
                    this.MatchingCharacter == ((Data)obj).MatchingCharacter &&
                    this.MatchingBracerType == ((Data)obj).MatchingBracerType &&
                    this.ExtendsLine == ((Data)obj).ExtendsLine &&
                    this.IgnoredBySyntax == ((Data)obj).IgnoredBySyntax;
            }
        }

        private static Dictionary<char, PapyrusDelimiter> allDelimiters = new Dictionary<char, PapyrusDelimiter>();

        /// <summary>
        /// An enumerable collection of all delimiters.
        /// </summary>
        public static IEnumerable<PapyrusDelimiter> All {
            get { return allDelimiters.Values; }
        }
        /// <summary>
        /// Retrieves a delimiter by the character that it represents.
        /// </summary>
        /// <param name="name">Returns null if delimiter does not exist.</param>
        /// <returns></returns>
        public static PapyrusDelimiter FromCharacter(char value) {
            PapyrusDelimiter delimiter;
            if (allDelimiters.TryGetValue(value, out delimiter)) {
                return delimiter;
            }
            return null;
        }

        #region Delimiter Definitions

        public static readonly PapyrusDelimiter Null = DefineDelimiter('\0', false, true);
        public static readonly PapyrusDelimiter LineFeed = DefineDelimiter('\n', false, true);
        public static readonly PapyrusDelimiter CarriageReturn = DefineDelimiter('\r', false, true);
        public static readonly PapyrusDelimiter Tab = DefineDelimiter('\t', false, true);
        public static readonly PapyrusDelimiter WhiteSpace = DefineDelimiter(' ', false, true);
        public static readonly PapyrusDelimiter ExclamationMark = DefineDelimiter('!', false, false);
        public static readonly PapyrusDelimiter QuotationMark = DefineDelimiter('"', false, false);
        public static readonly PapyrusDelimiter PercentSign = DefineDelimiter('%', false, false);
        public static readonly PapyrusDelimiter Ampersand = DefineDelimiter('&', false, false);
        public static readonly PapyrusDelimiter LeftRoundBracket = DefineDelimiter('(', ')', true);
        public static readonly PapyrusDelimiter RightRoundBracket = DefineDelimiter(')', '(', false);
        public static readonly PapyrusDelimiter Asterisk = DefineDelimiter('*', false, false);
        public static readonly PapyrusDelimiter PlusSign = DefineDelimiter('+', false, false);
        public static readonly PapyrusDelimiter Comma = DefineDelimiter(',', false, false);
        public static readonly PapyrusDelimiter Hyphen = DefineDelimiter('-', false, false);
        public static readonly PapyrusDelimiter FullStop = DefineDelimiter('.', false, false);
        public static readonly PapyrusDelimiter Slash = DefineDelimiter('/', false, false);
        public static readonly PapyrusDelimiter SemiColon = DefineDelimiter(';', false, false);
        public static readonly PapyrusDelimiter LessThanSign = DefineDelimiter('<', false, false);
        public static readonly PapyrusDelimiter EqualSign = DefineDelimiter('=', false, false);
        public static readonly PapyrusDelimiter GreaterThanSign = DefineDelimiter('>', false, false);
        public static readonly PapyrusDelimiter LeftSquareBracket = DefineDelimiter('[', ']', true);
        public static readonly PapyrusDelimiter RightSquareBracket = DefineDelimiter(']', '[', false);
        public static readonly PapyrusDelimiter LeftCurlyBracket = DefineDelimiter('{', false, false);
        public static readonly PapyrusDelimiter VerticalBar = DefineDelimiter('|', false, false);
        public static readonly PapyrusDelimiter RightCurlyBracket = DefineDelimiter('}', false, false);
        public static readonly PapyrusDelimiter Backslash = DefineDelimiter('\\', true, true);

        #endregion

        public PapyrusTokenType Type { get { return PapyrusTokenType.Delimiter; } }
        private Data data;
        public char Character { get { return data.Character; } }
        public int TokenSize { get { return 1; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsLineExtension { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }

        private PapyrusDelimiter(char character, char matchingCharacter, MatchingBracerType matchingBracerType, bool extendsLine, bool ignoredBySyntax) {
            data = new Data() {
                Character = character,
                MatchingCharacter = matchingCharacter,
                MatchingBracerType = matchingBracerType,
                ExtendsLine = extendsLine,
                IgnoredBySyntax = ignoredBySyntax,
            };
        }
        private PapyrusDelimiter(char character, bool extendsLine, bool ignoredBySyntax) :
            this(character, '\0', MatchingBracerType.None, extendsLine, ignoredBySyntax) {
        }
        private PapyrusDelimiter(char character, char matchingCharacter, bool isOpeningBracer) :
            this(character, matchingCharacter, isOpeningBracer ? MatchingBracerType.OpeningBracer : MatchingBracerType.ClosingBracer, false, false) {
        }
        private static PapyrusDelimiter DefineDelimiter(char character, char matchingCharacter, MatchingBracerType matchingBracerType, bool extendsLine, bool ignoredBySyntax) {
            PapyrusDelimiter delimiter = new PapyrusDelimiter(character, matchingCharacter, matchingBracerType, extendsLine, ignoredBySyntax);
            allDelimiters.Add(character, delimiter);
            return delimiter;
        }
        private static PapyrusDelimiter DefineDelimiter(char character, bool extendsLine, bool ignoredBySyntax) {
            return DefineDelimiter(character, Characters.Null, MatchingBracerType.None, extendsLine, ignoredBySyntax);
        }
        private static PapyrusDelimiter DefineDelimiter(char character, char matchingCharacter, bool isOpeningBracer) {
            return DefineDelimiter(character, matchingCharacter, isOpeningBracer ? MatchingBracerType.OpeningBracer : MatchingBracerType.ClosingBracer, false, false);
        }

        public static int FindNext(string source, int offset, params char[] anyOf) {
            int index = source.IndexOfAny(anyOf, offset);
            return index == -1 ? source.Length : index;
        }
        public static int FindNext(string source, int offset) {
            return FindNext(source, offset, allDelimiters.Keys.ToArray());
        }
        public static int FindNextExcluding(string source, int offset, params char[] noneOf) {
            return FindNext(source, offset, allDelimiters.Keys.Where(c => !noneOf.Contains(c)).ToArray());
        }
    }

    internal sealed class PapyrusDelimiterParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                PapyrusDelimiter delimiter = PapyrusDelimiter.FromCharacter(context.Source.FirstOrNull());
                if (delimiter != null) {
                    token = delimiter;
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
