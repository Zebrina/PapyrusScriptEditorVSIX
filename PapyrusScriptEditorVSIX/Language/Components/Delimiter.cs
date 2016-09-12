using Microsoft.VisualStudio.Package;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Components {
    public enum DelimiterSymbol : ushort {
        Null                = '\0',
        LineFeed            = '\n',
        CarriageReturn      = '\r',
        Tab                 = '\t',
        WhiteSpace          = ' ',
        //ExclamationMark     = '!',
        QuotationMark       = '"',
        //PercentSign         = '%',
        Ampersand           = '&',
        LeftRoundBracket    = '(',
        RightRoundBracket   = ')',
        //Asterisk            = '*',
        //PlusSign            = '+',
        Comma               = ',',
        //Hyphen              = '-',
        FullStop            = '.',
        //Slash               = '/',
        SemiColon           = ';',
        //LessThanSign        = '<',
        //EqualSign           = '=',
        //GreaterThanSign     = '>',
        LeftSquareBracket   = '[',
        RightSquareBracket  = ']',
        LeftCurlyBracket    = '{',
        VerticalBar         = '|',
        RightCurlyBracket   = '}',
        Backslash           = '\\',
    }

    public class Delimiter : TokenType {
        public const int TokenSize = 1;

        #region Delimiter Definitions

        private static readonly HashSet<char> AllDelimiters = new HashSet<char>();

        public static Delimiter Null { get { return new Delimiter(DelimiterSymbol.Null); } }
        public static Delimiter LineFeed { get { return new Delimiter(DelimiterSymbol.LineFeed); } }
        public static Delimiter CarriageReturn { get { return new Delimiter(DelimiterSymbol.CarriageReturn); } }
        public static Delimiter Tab { get { return new Delimiter(DelimiterSymbol.Tab); } }
        public static Delimiter WhiteSpace { get { return new Delimiter(DelimiterSymbol.WhiteSpace); } }
        //public static Delimiter ExclamationMark { get { return new Delimiter(DelimiterSymbol.ExclamationMark); } }
        public static Delimiter QuotationMark { get { return new Delimiter(DelimiterSymbol.QuotationMark); } }
        //public static Delimiter PercentSign { get { return new Delimiter(DelimiterSymbol.PercentSign); } }
        public static Delimiter Ampersand { get { return new Delimiter(DelimiterSymbol.Ampersand); } }
        public static Delimiter LeftRoundBracket { get { return new Delimiter(DelimiterSymbol.LeftRoundBracket); } }
        public static Delimiter RightRoundBracket { get { return new Delimiter(DelimiterSymbol.RightRoundBracket); } }
        //public static Delimiter Asterisk { get { return new Delimiter(DelimiterSymbol.Asterisk); } }
        //public static Delimiter PlusSign { get { return new Delimiter(DelimiterSymbol.PlusSign); } }
        public static Delimiter Comma { get { return new Delimiter(DelimiterSymbol.Comma); } }
        //public static Delimiter Hyphen { get { return new Delimiter(DelimiterSymbol.Hyphen); } }
        public static Delimiter FullStop { get { return new Delimiter(DelimiterSymbol.FullStop); } }
        //public static Delimiter Slash { get { return new Delimiter(DelimiterSymbol.Slash); } }
        public static Delimiter SemiColon { get { return new Delimiter(DelimiterSymbol.SemiColon); } }
        //public static Delimiter LessThanSign { get { return new Delimiter(DelimiterSymbol.LessThanSign); } }
        //public static Delimiter EqualSign { get { return new Delimiter(DelimiterSymbol.EqualSign); } }
        //public static Delimiter GreaterThanSign { get { return new Delimiter(DelimiterSymbol.GreaterThanSign); } }
        public static Delimiter LeftSquareBracket { get { return new Delimiter(DelimiterSymbol.LeftSquareBracket); } }
        public static Delimiter RightSquareBracket { get { return new Delimiter(DelimiterSymbol.RightSquareBracket); } }
        public static Delimiter LeftCurlyBracket { get { return new Delimiter(DelimiterSymbol.LeftCurlyBracket); } }
        public static Delimiter VerticalBar { get { return new Delimiter(DelimiterSymbol.VerticalBar); } }
        public static Delimiter RightCurlyBracket { get { return new Delimiter(DelimiterSymbol.RightCurlyBracket); } }
        public static Delimiter Backslash { get { return new Delimiter(DelimiterSymbol.Backslash); } }

        #endregion

        private DelimiterSymbol symbol;

        private Delimiter(DelimiterSymbol symbol) {
            this.symbol = symbol;
        }
        static Delimiter() {
            foreach (DelimiterSymbol symbol in Enum.GetValues(typeof(DelimiterSymbol))) {
                AllDelimiters.Add((char)symbol);
            }
        }

        public override string Text {
            get { return ((char)symbol).ToString(); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Delimiter; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Text; }
        }

        public override bool IsOpeningBracer() {
            return symbol == DelimiterSymbol.LeftRoundBracket || symbol == DelimiterSymbol.LeftSquareBracket;
        }
        public override bool IsClosingBracer(TokenType openingBracer) {
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
        public override bool IgnoredInLine {
            get { return symbol == DelimiterSymbol.Backslash; }
        }

        public override int GetHashCode() {
            return symbol.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Delimiter && this.symbol == ((Delimiter)obj).symbol;
        }

        public static explicit operator char(Delimiter delimiter) {
            return (char)delimiter.symbol;
        }

        public static bool operator ==(Delimiter x, Delimiter y) {
            return Equals(x, y);
        }
        public static bool operator !=(Delimiter x, Delimiter y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Delimiter x, TokenType y) {
            return x == (Delimiter)y;
        }
        public static bool operator !=(Delimiter x, TokenType y) {
            return x != (Delimiter)y;
        }

        public static Delimiter Parse(char c) {
            if (IsValidDelimiterSymbol(c)) {
                return new Delimiter((DelimiterSymbol)c);
            }
            return null;
        }

        private static bool IsValidDelimiterSymbol(char symbol) {
            return AllDelimiters.Contains(symbol);
        }

        [DebuggerStepThrough]
        public static int FindNext(string source, int offset, params char[] anyOf) {
            if (offset >= 0 && offset < source.Length) {
                ICollection<char> delimiters;
                if (anyOf.Count() > 0) {
                    delimiters = anyOf;
                }
                else {
                    delimiters = AllDelimiters;
                }

                for (int i = offset; i < source.Length; ++i) {
                    if (delimiters.Contains(source[i])) {
                        return i;
                    }
                }
            }
            return source.Length;
        }
        [DebuggerStepThrough]
        public static int FindPrevious(string source, int offset, params char[] anyOf) {
            if (offset >= 0 && offset < source.Length) {
                ICollection<char> delimiters;
                if (anyOf.Count() > 0) {
                    delimiters = anyOf;
                }
                else {
                    delimiters = AllDelimiters;
                }

                for (int i = offset; i >= 0; --i) {
                    if (delimiters.Contains(source[i])) {
                        return i;
                    }
                }
            }
            return source.Length;
        }

        [Obsolete]
        [DebuggerStepThrough]
        public static int FindNextWhiteSpace(string source, int offset) {
            int position;
            for (position = offset; position < source.Length; ++position) {
                if (Char.IsWhiteSpace(source[position])) {
                    break;
                }
            }

            return position;
        }

        [Obsolete]
        [DebuggerStepThrough]
        public static int FindNextNonWhiteSpace(string source, int offset) {
            int position;
            for (position = offset; position < source.Length; ++position) {
                if (!Char.IsWhiteSpace(source[position])) {
                    break;
                }
            }

            return position;
        }
    }
}
