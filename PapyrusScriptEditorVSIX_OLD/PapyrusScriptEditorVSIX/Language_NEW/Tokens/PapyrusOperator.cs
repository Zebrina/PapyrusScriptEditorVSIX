using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Language_NEW.Data;
using Papyrus.Language_NEW.Parsing;
using Papyrus.Language_NEW.Parsing.Interfaces;
using Papyrus.Language_NEW.Tokens;
using Papyrus.Language_NEW.Tokens.Interfaces;
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
namespace Papyrus.Language_NEW.Tokens {
    public class PapyrusOperator : IPapyrusToken, ISyntaxColorableToken {
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

        public PapyrusTokenType Type { get { return PapyrusTokenType.Keyword; } }
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

        public bool IsEqualToToken(IPapyrusToken other) {
            return ReferenceEquals(this, other);
        }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo textFormatInfo) {
            stringBuilder.Append(Value);
            return true;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusOperatorColorFormat.Name);
        }

        #region Parsing

        public static PapyrusOperator Parse(string source, int position = 0) {
            foreach (var op in allOperators) {
                if (op.Value.Value.Equals(source.Substring(position, op.Value.TokenSize))) {
                    return op.Value;
                }
            }
            return null;
        }

        #endregion
    }

    internal sealed class PapyrusOperatorParser : ITokenParser {
        public bool TryParse(string sourceTextSpan, TokenScanner scanner, IEnumerable<IPapyrusToken> previousTokens, out IPapyrusToken token) {
            if (scanner.CurrentState == TokenScannerState.Default) {
                PapyrusOperator op = PapyrusOperator.Parse(sourceTextSpan);
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
