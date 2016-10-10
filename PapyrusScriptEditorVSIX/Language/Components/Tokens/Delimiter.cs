using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Papyrus.Language.Components.Tokens {
    public enum DelimiterSymbol {
        Null                = '\0',
        LineFeed            = '\n',
        CarriageReturn      = '\r',
        Tab                 = '\t',
        WhiteSpace          = ' ',
        ExclamationMark     = '!',
        QuotationMark       = '"',
        PercentSign         = '%',
        Ampersand           = '&',
        LeftRoundBracket    = '(',
        RightRoundBracket   = ')',
        Asterisk            = '*',
        PlusSign            = '+',
        Comma               = ',',
        Hyphen              = '-',
        FullStop            = '.',
        Slash               = '/',
        SemiColon           = ';',
        LessThanSign        = '<',
        EqualSign           = '=',
        GreaterThanSign     = '>',
        LeftSquareBracket   = '[',
        RightSquareBracket  = ']',
        LeftCurlyBracket    = '{',
        VerticalBar         = '|',
        RightCurlyBracket   = '}',
        Backslash           = '\\',
    }

    [DebuggerStepThrough]
    public class Delimiter : Token {
        public const int TokenSize = 1;

        #region Delimiter Definitions

        private static readonly IDictionary<DelimiterSymbol, Delimiter> allDelimiters =
            Enum.GetValues(typeof(DelimiterSymbol)).Cast<DelimiterSymbol>().Select(s => new Delimiter(s)).ToDictionary(s => s.symbol);
        public ICollection<Delimiter> All {
            get { return allDelimiters.Values; }
        }

        public static Delimiter Null { get { return allDelimiters[DelimiterSymbol.Null]; } }
        public static Delimiter LineFeed { get { return allDelimiters[DelimiterSymbol.LineFeed]; } }
        public static Delimiter CarriageReturn { get { return allDelimiters[DelimiterSymbol.CarriageReturn]; } }
        public static Delimiter Tab { get { return allDelimiters[DelimiterSymbol.Tab]; } }
        public static Delimiter WhiteSpace { get { return allDelimiters[DelimiterSymbol.WhiteSpace]; } }
        public static Delimiter ExclamationMark { get { return allDelimiters[DelimiterSymbol.ExclamationMark]; } }
        public static Delimiter QuotationMark { get { return allDelimiters[DelimiterSymbol.QuotationMark]; } }
        public static Delimiter PercentSign { get { return allDelimiters[DelimiterSymbol.PercentSign]; } }
        public static Delimiter Ampersand { get { return allDelimiters[DelimiterSymbol.Ampersand]; } }
        public static Delimiter LeftRoundBracket { get { return allDelimiters[DelimiterSymbol.LeftRoundBracket]; } }
        public static Delimiter RightRoundBracket { get { return allDelimiters[DelimiterSymbol.RightRoundBracket]; } }
        public static Delimiter Asterisk { get { return allDelimiters[DelimiterSymbol.Asterisk]; } }
        public static Delimiter PlusSign { get { return allDelimiters[DelimiterSymbol.PlusSign]; } }
        public static Delimiter Comma { get { return allDelimiters[DelimiterSymbol.Comma]; } }
        public static Delimiter Hyphen { get { return allDelimiters[DelimiterSymbol.Hyphen]; } }
        public static Delimiter FullStop { get { return allDelimiters[DelimiterSymbol.FullStop]; } }
        public static Delimiter Slash { get { return allDelimiters[DelimiterSymbol.Slash]; } }
        public static Delimiter SemiColon { get { return allDelimiters[DelimiterSymbol.SemiColon]; } }
        public static Delimiter LessThanSign { get { return allDelimiters[DelimiterSymbol.LessThanSign]; } }
        public static Delimiter EqualSign { get { return allDelimiters[DelimiterSymbol.EqualSign]; } }
        public static Delimiter GreaterThanSign { get { return allDelimiters[DelimiterSymbol.GreaterThanSign]; } }
        public static Delimiter LeftSquareBracket { get { return allDelimiters[DelimiterSymbol.LeftSquareBracket]; } }
        public static Delimiter RightSquareBracket { get { return allDelimiters[DelimiterSymbol.RightSquareBracket]; } }
        public static Delimiter LeftCurlyBracket { get { return allDelimiters[DelimiterSymbol.LeftCurlyBracket]; } }
        public static Delimiter VerticalBar { get { return allDelimiters[DelimiterSymbol.VerticalBar]; } }
        public static Delimiter RightCurlyBracket { get { return allDelimiters[DelimiterSymbol.RightCurlyBracket]; } }
        public static Delimiter Backslash { get { return allDelimiters[DelimiterSymbol.Backslash]; } }

        #endregion

        private DelimiterSymbol symbol;

        private Delimiter(DelimiterSymbol symbol) {
            this.symbol = symbol;
        }

        public override string Text {
            get { return ((char)symbol).ToString(); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Delimiter; }
        }

        public override bool IsOpeningBracer() {
            return symbol == DelimiterSymbol.LeftRoundBracket || symbol == DelimiterSymbol.LeftSquareBracket;
        }
        public override bool IsClosingBracer(Token openingBracer) {
            if (openingBracer is Delimiter) {
                switch (((Delimiter)openingBracer).symbol) {
                    case DelimiterSymbol.RightRoundBracket:
                        return this.symbol == DelimiterSymbol.LeftRoundBracket;
                    case DelimiterSymbol.RightSquareBracket:
                        return this.symbol == DelimiterSymbol.LeftSquareBracket;
                }
            }
            return false;
        }

        public override bool ExtendsLine {
            get { return symbol == DelimiterSymbol.Backslash; }
        }
        public override bool IgnoredInSyntax {
            get { return symbol == DelimiterSymbol.Backslash; }
        }

        public override int GetHashCode() {
            return symbol.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Delimiter && this.symbol == ((Delimiter)obj).symbol;
        }

        public static implicit operator char(Delimiter delimiter) {
            return (char)delimiter.symbol;
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

        public static Delimiter Parse(char c) {
            if (IsValidDelimiterSymbol(c)) {
                return new Delimiter((DelimiterSymbol)c);
            }
            return null;
        }

        private static bool IsValidDelimiterSymbol(char symbol) {
            return allDelimiters.ContainsKey((DelimiterSymbol)symbol);
        }
        
        private static int FindNext(string source, int offset, IEnumerable<DelimiterSymbol> anyOf) {
            int index = 0;
            source.Any(c => {
                if (index >= offset && anyOf.Contains((DelimiterSymbol)c)) {
                    return true;
                }
                ++index;
                return false;
            });
            return index;
        }
        public static int FindNext(string source, int offset) {
            return FindNext(source, offset, allDelimiters.Keys);
        }
        public static int FindNext(string source, int offset, params Delimiter[] anyOf) {
            return FindNext(source, offset, anyOf.Select(d => d.symbol));
        }
        public static int FindNextExcluding(string source, int offset, params Delimiter[] noneOf) {
            var set = allDelimiters.SelectWhere<KeyValuePair<DelimiterSymbol, Delimiter>, DelimiterSymbol, HashSet<DelimiterSymbol>>(d => !noneOf.Contains(d.Value), d => d.Key);
            return FindNext(source, offset, set);
        }
    }

    internal sealed class DelimiterParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                Delimiter delimiter = Delimiter.Parse(sourceTextSpan.First());
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
