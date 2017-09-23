using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using Papyrus.Language.Parsing;
using Papyrus.Language.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusOperator : IPapyrusToken, ISyntaxColorableToken {
        private enum OperatorType {
            Prefix,
        }

        private class StringComparer : IComparer<string> {
            int IComparer<string>.Compare(string x, string y) {
                if (x.Length != y.Length) {
                    x.Length.CompareTo(y.Length);
                }
                return x.CompareTo(y);
            }
        }

        private static SortedList<string, PapyrusOperator> allOperators = new SortedList<string, PapyrusOperator>(new StringComparer());

        /// <summary>
        /// An enumerable collection of all operators.
        /// </summary>
        public static IEnumerable<PapyrusOperator> All {
            get { return allOperators.Values; }
        }

        #region Operator definitions

        public static readonly PapyrusOperator BasicAssignment = DefineOperator("=");
        public static readonly PapyrusOperator Addition = DefineOperator("+");
        public static readonly PapyrusOperator Subtraction = DefineOperator("-");
        public static readonly PapyrusOperator Multiplication = DefineOperator("*");
        public static readonly PapyrusOperator Division = DefineOperator("/");
        public static readonly PapyrusOperator Modulo = DefineOperator("%");
        public static readonly PapyrusOperator EqualTo = DefineOperator("==");
        public static readonly PapyrusOperator NotEqualTo = DefineOperator("!=");
        public static readonly PapyrusOperator GreaterThan = DefineOperator(">");
        public static readonly PapyrusOperator LessThan = DefineOperator("<");
        public static readonly PapyrusOperator GreaterThanOrEqualTo = DefineOperator(">=");
        public static readonly PapyrusOperator LessThanOrEqualTo = DefineOperator("<=");
        public static readonly PapyrusOperator LogicalNOT = DefineOperator("!");
        public static readonly PapyrusOperator LogicalAND = DefineOperator("&&");
        public static readonly PapyrusOperator LogicalOR = DefineOperator("||");
        public static readonly PapyrusOperator AdditionAssignment = DefineOperator("+=");
        public static readonly PapyrusOperator SubtractionAssignment = DefineOperator("-=");
        public static readonly PapyrusOperator MultiplicationAssignment = DefineOperator("*=");
        public static readonly PapyrusOperator DivisionAssignment = DefineOperator("/=");
        public static readonly PapyrusOperator ModuloAssignment = DefineOperator("%=");
        public static readonly PapyrusOperator StructureReference = DefineOperator(".");

        // Fallout 4
        //public static readonly PapyrusOperator Namespace = DefineOperator(":");

        #endregion

        public PapyrusTokenType Type { get { return PapyrusTokenType.Operator; } }
        public string Value { get; private set; }
        public int TokenSize { get { return Value.Length; } }
        public bool IsCompileTimeConstant { get { return false; } }
        public bool IsLineExtension { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }

        private PapyrusOperator(string value) {
            this.Value = value;
        }
        private static PapyrusOperator DefineOperator(string value) {
            PapyrusOperator ppsOperator = new PapyrusOperator(value);
            allOperators.Add(value, ppsOperator);
            return ppsOperator;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusOperatorColorFormat.Name);
        }

        #region Parsing

        public static PapyrusOperator Parse(string source, int position = 0) {
            if (!String.IsNullOrEmpty(source)) {
                foreach (var op in allOperators.Values) {
                    if (source.Length - position >= op.TokenSize && op.Value.Equals(source.Substring(position, op.TokenSize))) {
                        return op;
                    }
                }
            }
            return null;
        }

        #endregion
    }

    internal sealed class PapyrusOperatorParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                PapyrusOperator op = PapyrusOperator.Parse(context.Source);
                if (op != null) {
                    token = op;
                    return true;
                }
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
    internal sealed class PapyrusOperatorColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusOperator";

        internal PapyrusOperatorColorFormat() {
            DisplayName = "Papyrus Operator";
            //ForegroundColor = Color.FromRgb(214, 157, 133);
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusOperatorColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusOperatorColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
