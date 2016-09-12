using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Papyrus.Language.Components {
    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class OperatorColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusOperator";

        internal OperatorColorFormat() {
            DisplayName = "Papyrus Operator";
            //ForegroundColor = Color.FromRgb(214, 157, 133);
        }
    }

    internal static class OperatorColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(OperatorColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    public enum OperatorSymbol : uint {
        BasicAssignment             = '=',
        Addition                    = '+',
        Subtraction                 = '-',
        //UnaryPlus                   = '+',
        //UnaryMinus                  = '-',
        Multiplication              = '*',
        Division                    = '/',
        Modulo                      = '%',
        EqualTo                     = '=' | ('=' << 16),
        NotEqualTo                  = '!' | ('=' << 16),
        GreaterThan                 = '>',
        LessThan                    = '<',
        GreaterThanOrEqualTo        = '>' | ('=' << 16),
        LessThanOrEqualTo           = '<' | ('=' << 16),
        LogicalNOT                  = '!',
        LogicalAND                  = '&' | ('&' << 16),
        LogicalOR                   = '|' | ('|' << 16),
        AdditionAssignment          = '+' | ('=' << 16),
        SubtractionAssignment       = '-' | ('=' << 16),
        MultiplicationAssignment    = '*' | ('=' << 16),
        DivisionAssignment          = '/' | ('=' << 16),
        ModuloAssignment            = '%' | ('=' << 16),
        StructureReference          = '.',
    }

    public sealed class Operator : TokenType, ISyntaxColorable {
        #region Operator Definitions

        private static readonly Dictionary<OperatorSymbol, Operator> collection = new Dictionary<OperatorSymbol, Operator>() {
            { OperatorSymbol.BasicAssignment, new Operator(OperatorSymbol.BasicAssignment) },
            { OperatorSymbol.Addition, new Operator(OperatorSymbol.Addition) },
            { OperatorSymbol.Subtraction, new Operator(OperatorSymbol.Subtraction) },
            { OperatorSymbol.Multiplication, new Operator(OperatorSymbol.Multiplication) },
            { OperatorSymbol.Division, new Operator(OperatorSymbol.Division) },
            { OperatorSymbol.Modulo, new Operator(OperatorSymbol.Modulo) },
            { OperatorSymbol.EqualTo, new Operator(OperatorSymbol.EqualTo) },
            { OperatorSymbol.NotEqualTo, new Operator(OperatorSymbol.NotEqualTo) },
            { OperatorSymbol.GreaterThan, new Operator(OperatorSymbol.GreaterThan) },
            { OperatorSymbol.LessThan, new Operator(OperatorSymbol.LessThan) },
            { OperatorSymbol.GreaterThanOrEqualTo, new Operator(OperatorSymbol.GreaterThanOrEqualTo) },
            { OperatorSymbol.LessThanOrEqualTo, new Operator(OperatorSymbol.LessThanOrEqualTo) },
            { OperatorSymbol.LogicalNOT, new Operator(OperatorSymbol.LogicalNOT) },
            { OperatorSymbol.LogicalAND, new Operator(OperatorSymbol.LogicalAND) },
            { OperatorSymbol.LogicalOR, new Operator(OperatorSymbol.LogicalOR) },
            { OperatorSymbol.AdditionAssignment, new Operator(OperatorSymbol.AdditionAssignment) },
            { OperatorSymbol.SubtractionAssignment, new Operator(OperatorSymbol.SubtractionAssignment) },
            { OperatorSymbol.MultiplicationAssignment, new Operator(OperatorSymbol.MultiplicationAssignment) },
            { OperatorSymbol.DivisionAssignment, new Operator(OperatorSymbol.DivisionAssignment) },
            { OperatorSymbol.ModuloAssignment, new Operator(OperatorSymbol.ModuloAssignment) },
            { OperatorSymbol.StructureReference, new Operator(OperatorSymbol.StructureReference) },
        };

        public static Operator BasicAssignment { get { return collection[OperatorSymbol.BasicAssignment]; } }
        public static Operator Addition { get { return collection[OperatorSymbol.Addition]; } }
        public static Operator Subtraction { get { return collection[OperatorSymbol.Subtraction]; } }
        public static Operator Multiplication { get { return collection[OperatorSymbol.Multiplication]; } }
        public static Operator Division { get { return collection[OperatorSymbol.Division]; } }
        public static Operator Modulo { get { return collection[OperatorSymbol.Modulo]; } }
        public static Operator EqualTo { get { return collection[OperatorSymbol.EqualTo]; } }
        public static Operator NotEqualTo { get { return collection[OperatorSymbol.NotEqualTo]; } }
        public static Operator GreaterThan { get { return collection[OperatorSymbol.GreaterThan]; } }
        public static Operator LessThan { get { return collection[OperatorSymbol.LessThan]; } }
        public static Operator GreaterThanOrEqualTo { get { return collection[OperatorSymbol.GreaterThanOrEqualTo]; } }
        public static Operator LessThanOrEqualTo { get { return collection[OperatorSymbol.LessThanOrEqualTo]; } }
        public static Operator LogicalNOT { get { return collection[OperatorSymbol.LogicalNOT]; } }
        public static Operator LogicalAND { get { return collection[OperatorSymbol.LogicalAND]; } }
        public static Operator LogicalOR { get { return collection[OperatorSymbol.LogicalOR]; } }
        public static Operator AdditionAssignment { get { return collection[OperatorSymbol.AdditionAssignment]; } }
        public static Operator SubtractionAssignment { get { return collection[OperatorSymbol.SubtractionAssignment]; } }
        public static Operator MultiplicationAssignment { get { return collection[OperatorSymbol.MultiplicationAssignment]; } }
        public static Operator DivisionAssignment { get { return collection[OperatorSymbol.DivisionAssignment]; } }
        public static Operator ModuloAssignment { get { return collection[OperatorSymbol.ModuloAssignment]; } }
        public static Operator StructureReference { get { return collection[OperatorSymbol.StructureReference]; } }

        #endregion

        private OperatorSymbol symbol;

        private Operator(OperatorSymbol symbol) {
            this.symbol = symbol;
        }

        public override string Text {
            get { return Encoding.Unicode.GetString(BitConverter.GetBytes((uint)symbol)).TrimEnd('\0'); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Operator; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Text; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(OperatorColorFormat.Name);
        }

        public override int GetHashCode() {
            return symbol.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Operator && this.symbol == ((Operator)obj).symbol;
        }

        public static implicit operator string(Operator op) {
            return op.Text;
        }

        public static implicit operator Predicate<TokenType>(Operator op) {
            return token => { return token.Equals(op); };
        }

        public static bool operator ==(Operator x, Operator y) {
            return Equals(x, y);
        }
        public static bool operator !=(Operator x, Operator y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Operator x, TokenType y) {
            return x == (Operator)y;
        }
        public static bool operator !=(Operator x, TokenType y) {
            return x != (Operator)y;
        }

        public static Operator Parse(string source, int offset) {
            uint symbol = 0;
            for (int i = offset; i < source.Length; ++i) {
                if (!IsValidCharacter(source[i])) {
                    break;
                }
                symbol = (symbol << 8) | source[i];
            }

            Operator o;
            if (collection.TryGetValue((OperatorSymbol)symbol, out o)) {
                return o;
            }
            return null;
        }

        private static bool IsValidCharacter(char value) {
            switch (value) {
                case '=':
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '!':
                case '>':
                case '<':
                case '&':
                case '|':
                case '.':
                    return true;
                default:
                    return false;
            }
        }
    }
}

/*
 *         [Description("=")]

        LogicalOR,
        [Description("+=")]
        AdditionAssignment,
        [Description("-=")]
        SubtractionAssignment,
        [Description("*=")]
        MultiplicationAssignment,
        [Description("/=")]
        DivisionAssignment,
        [Description("%=")]
        ModuloAssignment,
        [Description(".")]
*/