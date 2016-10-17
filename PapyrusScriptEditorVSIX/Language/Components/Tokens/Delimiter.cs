using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Components.Tokens {
    //[DebuggerStepThrough]
    public class Delimiter : Token, IKeyByValue<char> {
        public const int TokenSize = 1;

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

        #region Delimiter Definitions

        public static Delimiter Null { get { return Table[Characters.Null]; } }
        public static Delimiter LineFeed { get { return Table[Characters.LineFeed]; } }
        public static Delimiter CarriageReturn { get { return Table[Characters.CarriageReturn]; } }
        public static Delimiter Tab { get { return Table[Characters.Tab]; } }
        public static Delimiter WhiteSpace { get { return Table[Characters.WhiteSpace]; } }
        public static Delimiter ExclamationMark { get { return Table[Characters.ExclamationMark]; } }
        public static Delimiter QuotationMark { get { return Table[Characters.QuotationMark]; } }
        public static Delimiter PercentSign { get { return Table[Characters.PercentSign]; } }
        public static Delimiter Ampersand { get { return Table[Characters.Ampersand]; } }
        public static Delimiter LeftRoundBracket { get { return Table[Characters.LeftRoundBracket]; } }
        public static Delimiter RightRoundBracket { get { return Table[Characters.RightRoundBracket]; } }
        public static Delimiter Asterisk { get { return Table[Characters.Asterisk]; } }
        public static Delimiter PlusSign { get { return Table[Characters.PlusSign]; } }
        public static Delimiter Comma { get { return Table[Characters.Comma]; } }
        public static Delimiter Hyphen { get { return Table[Characters.Hyphen]; } }
        public static Delimiter FullStop { get { return Table[Characters.FullStop]; } }
        public static Delimiter Slash { get { return Table[Characters.Slash]; } }
        public static Delimiter SemiColon { get { return Table[Characters.SemiColon]; } }
        public static Delimiter LessThanSign { get { return Table[Characters.LessThanSign]; } }
        public static Delimiter EqualSign { get { return Table[Characters.EqualSign]; } }
        public static Delimiter GreaterThanSign { get { return Table[Characters.GreaterThanSign]; } }
        public static Delimiter LeftSquareBracket { get { return Table[Characters.LeftSquareBracket]; } }
        public static Delimiter RightSquareBracket { get { return Table[Characters.RightSquareBracket]; } }
        public static Delimiter LeftCurlyBracket { get { return Table[Characters.LeftCurlyBracket]; } }
        public static Delimiter VerticalBar { get { return Table[Characters.VerticalBar]; } }
        public static Delimiter RightCurlyBracket { get { return Table[Characters.RightCurlyBracket]; } }
        public static Delimiter Backslash { get { return Table[Characters.Backslash]; } }

        private static Dictionary<char, Delimiter> table = null;
        private static object tableSync = new object();
        private static IReadOnlyDictionary<char, Delimiter> Table {
            get {
                if (table == null) {
                    lock (tableSync) {
                        if (table == null) {
                            table = new Dictionary<char, Delimiter>() {
                                new Delimiter(Characters.Null, false, true),
                                new Delimiter(Characters.LineFeed, false, true),
                                new Delimiter(Characters.CarriageReturn, false, true),
                                new Delimiter(Characters.Tab, false, true),
                                new Delimiter(Characters.WhiteSpace, false, true),
                                new Delimiter(Characters.ExclamationMark, false, false),
                                new Delimiter(Characters.QuotationMark, false, false),
                                new Delimiter(Characters.PercentSign, false, false),
                                new Delimiter(Characters.Ampersand, false, false),
                                new Delimiter(Characters.LeftRoundBracket, Characters.RightRoundBracket, true),
                                new Delimiter(Characters.RightRoundBracket, Characters.LeftRoundBracket, false),
                                new Delimiter(Characters.Asterisk, false, false),
                                new Delimiter(Characters.PlusSign, false, false),
                                new Delimiter(Characters.Comma, false, false),
                                new Delimiter(Characters.Hyphen, false, false),
                                new Delimiter(Characters.FullStop, false, false),
                                new Delimiter(Characters.Slash, false, false),
                                new Delimiter(Characters.SemiColon, false, false),
                                new Delimiter(Characters.LessThanSign, false, false),
                                new Delimiter(Characters.EqualSign, false, false),
                                new Delimiter(Characters.GreaterThanSign, false, false),
                                new Delimiter(Characters.LeftSquareBracket, Characters.RightSquareBracket, true),
                                new Delimiter(Characters.RightSquareBracket, Characters.LeftSquareBracket, false),
                                new Delimiter(Characters.LeftCurlyBracket, false, false),
                                new Delimiter(Characters.VerticalBar, false, false),
                                new Delimiter(Characters.RightCurlyBracket, false, false),
                            };
                        }
                    }
                }
                return table;
            }
        }

        #endregion

        public static IEnumerable<Delimiter> All {
            get { return Table.Values; }
        }

        private Data data;

        private Delimiter(char character, char matchingCharacter, MatchingBracerType matchingBracerType, bool extendsLine, bool ignoredBySyntax) {
            data = new Data() {
                Character = character,
                MatchingCharacter = matchingCharacter,
                MatchingBracerType = matchingBracerType,
                ExtendsLine = extendsLine,
                IgnoredBySyntax = ignoredBySyntax,
            };
        }
        private Delimiter(char character, bool extendsLine, bool ignoredBySyntax) :
            this(character, Characters.Null, MatchingBracerType.None, extendsLine, ignoredBySyntax) {
        }
        private Delimiter(char character, char matchingCharacter, bool isOpeningBracer) :
            this(character, matchingCharacter, isOpeningBracer ? MatchingBracerType.OpeningBracer : MatchingBracerType.ClosingBracer, false, false) {
        }

        public override string Text {
            get { return data.Character.ToString(); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Delimiter; }
        }

        public override bool IsOpeningBracer {
            get { return this.data.MatchingBracerType == MatchingBracerType.OpeningBracer; }
        }
        public override bool IsClosingBracer {
            get { return this.data.MatchingBracerType == MatchingBracerType.ClosingBracer; }
        }
        public override bool MatchesWithBracer(Token otherBracer) {
            return otherBracer is Delimiter && ((Delimiter)otherBracer).data.Character == this.data.MatchingCharacter;
        }

        public override bool ExtendsLine {
            get { return data.ExtendsLine; }
        }
        public override bool IgnoredBySyntax {
            get { return data.IgnoredBySyntax; }
        }

        char IKeyByValue<char>.Key {
            get { return data.Character; }
        }

        public override int GetHashCode() {
            return data.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Delimiter && this.data.Equals(((Delimiter)obj).data);
        }

        public static bool operator ==(Delimiter x, Delimiter y) {
            return Equals(x, y);
        }
        public static bool operator !=(Delimiter x, Delimiter y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Delimiter x, Token y) {
            return x == y as Delimiter;
        }
        public static bool operator !=(Delimiter x, Token y) {
            return x == y as Delimiter;
        }
        public static bool operator ==(Token x, Delimiter y) {
            return x as Delimiter == y;
        }
        public static bool operator !=(Token x, Delimiter y) {
            return x as Delimiter != y;
        }

        public static Delimiter FromChar(char c) {
            Delimiter delimiter;
            if (Table.TryGetValue(c, out delimiter)) {
                return delimiter;
            }
            return null;
        }

        public static int FindNext(string source, int offset, params char[] anyOf) {
            int index = source.IndexOfAny(anyOf, offset);
            return index == -1 ? source.Length : index;
        }
        public static int FindNext(string source, int offset) {
            return FindNext(source, offset, Table.Keys.ToArray());
        }
        public static int FindNextExcluding(string source, int offset, params char[] noneOf) {
            return FindNext(source, offset, Table.Keys.Where(c => !noneOf.Contains(c)).ToArray());
        }
    }

    internal sealed class DelimiterParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                Delimiter delimiter = Delimiter.FromChar(sourceTextSpan.First());
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
