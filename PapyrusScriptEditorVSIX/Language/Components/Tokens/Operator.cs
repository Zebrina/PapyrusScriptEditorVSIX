using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Papyrus.Language.Components.Tokens {
    [DebuggerStepThrough]
    internal static class OperatorColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(OperatorColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

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

    public enum OperatorSymbol {
        [Description("=")]
        BasicAssignment,
        [Description("+")]
        Addition,
        [Description("-")]
        Subtraction,
        [Description("*")]
        Multiplication,
        [Description("/")]
        Division,
        [Description("%")]
        Modulo,
        [Description("==")]
        EqualTo,
        [Description("!=")]
        NotEqualTo,
        [Description(">")]
        GreaterThan,
        [Description("<")]
        LessThan,
        [Description(">=")]
        GreaterThanOrEqualTo,
        [Description("<=")]
        LessThanOrEqualTo,
        [Description("!")]
        LogicalNOT,
        [Description("&&")]
        LogicalAND,
        [Description("||")]
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
        StructureReference,
    }

    public sealed class Operator : Token, ISyntaxColorable, IComparable<Operator> {
        #region Operator Definitions

        private static readonly SortedList<OperatorSymbol, Operator> collection = new SortedList<OperatorSymbol, Operator>() {
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

        public static IReadOnlyCollection<Operator> All {
            get { return (IReadOnlyCollection<Operator>)collection.Values; }
        }

        #endregion

        private OperatorSymbol symbol;

        private Operator(OperatorSymbol symbol) {
            this.symbol = symbol;
        }

        public override string Text {
            get { return symbol.GetDescription(typeof(OperatorSymbol)); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Operator; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(OperatorColorFormat.Name);
        }
        
        public int CompareTo(Operator other) {
            return -1 * this.Text.CompareTo(other.Text);
        }

        public override string ToString() {
            return Text;
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

        public static bool operator ==(Operator x, Operator y) {
            return Equals(x, y);
        }
        public static bool operator !=(Operator x, Operator y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Operator x, Token y) {
            return x == y as Operator;
        }
        public static bool operator !=(Operator x, Token y) {
            return x != y as Operator;
        }
        public static bool operator ==(Token x, Operator y) {
            return x as Operator == y;
        }
        public static bool operator !=(Token x, Operator y) {
            return x as Operator != y;
        }

        public static Operator Parse(string source, int offset) {
            foreach (var op in collection) {
                string text = op.Key.GetDescription(typeof(OperatorSymbol));
                if (String.Compare(source, 0, text, 0, text.Length) == 0) {
                    return op.Value;
                }
            }
            return null;
        }
    }

    internal sealed class OperatorParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            if (state == TokenScannerState.Text) {
                Operator op = Operator.Parse(sourceSnapshotSpan.GetText(), 0);
                if (op != null) {
                    token.Type = op;
                    token.Span = sourceSnapshotSpan.Subspan(0, op.Text.Length);
                    return true;
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                Operator op = Operator.Parse(sourceTextSpan, 0);
                if (op != null) {
                    token = op;
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
