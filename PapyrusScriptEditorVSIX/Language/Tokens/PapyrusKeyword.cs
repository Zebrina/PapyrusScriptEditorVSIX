using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Common.Extensions;
using Papyrus.Language.Parsing;
using Papyrus.Language.Tokens.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Media;

// READY
namespace Papyrus.Language.Tokens {
    public sealed class PapyrusKeyword : IPapyrusToken, ISyntaxColorableToken {
        public enum KeywordAttribute {
            ScriptAttribute,
            PropertyAttribute,
            FieldAttribute,
            FunctionAttribute,
            EventAttribute,
            StateAttribute,
            NativeType,
            CompileTimeConstant,
            SpecialIdentifier,
            SpecialMember,
            SpecialOperator,
            OutlineableBegin,
            OutlineableEnd,
            IndentIncreaseAtNewLine,
            IndentDecrease,
            BreakOutlineableProperty,
            BreakOutlineableFunction,
        }

        private static Dictionary<string, PapyrusKeyword> allKeywords = new Dictionary<string, PapyrusKeyword>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// An enumerable collection of all keywords.
        /// </summary>
        public static IEnumerable<PapyrusKeyword> All {
            get { return allKeywords.Values; }
        }
        /// <summary>
        /// Retrieves a keyword by it's name.
        /// </summary>
        /// <param name="name">Returns null if keyword does not exist.</param>
        /// <returns></returns>
        public static PapyrusKeyword FromName(string name) {
            PapyrusKeyword keyword;
            if (allKeywords.TryGetValue(name, out keyword)) {
                return keyword;
            }
            return null;
        }

        #region Skyrim Keyword Definitions

        // Definition
        public static readonly PapyrusKeyword Scriptname = DefineKeyword("Scriptname");
        public static readonly PapyrusKeyword Extends = DefineKeyword("Extends");
        public static readonly PapyrusKeyword Import = DefineKeyword("Import");

        // Attributes
        public static readonly PapyrusKeyword Hidden = DefineKeyword("Hidden", KeywordAttribute.ScriptAttribute, KeywordAttribute.PropertyAttribute);
        public static readonly PapyrusKeyword Conditional = DefineKeyword("Conditional", KeywordAttribute.ScriptAttribute, KeywordAttribute.PropertyAttribute, KeywordAttribute.FieldAttribute);
        public static readonly PapyrusKeyword Global = DefineKeyword("Global", KeywordAttribute.FunctionAttribute);
        public static readonly PapyrusKeyword Auto = DefineKeyword("Auto", KeywordAttribute.PropertyAttribute, KeywordAttribute.StateAttribute, KeywordAttribute.BreakOutlineableProperty);
        public static readonly PapyrusKeyword AutoReadOnly = DefineKeyword("AutoReadOnly", KeywordAttribute.PropertyAttribute, KeywordAttribute.BreakOutlineableProperty);
        public static readonly PapyrusKeyword Native = DefineKeyword("Native", KeywordAttribute.FunctionAttribute, KeywordAttribute.BreakOutlineableFunction);

        // Native types
        public static readonly PapyrusKeyword Bool = DefineKeyword("Bool", KeywordAttribute.NativeType);
        public static readonly PapyrusKeyword Int = DefineKeyword("Int", KeywordAttribute.NativeType);
        public static readonly PapyrusKeyword Float = DefineKeyword("Float", KeywordAttribute.NativeType);
        public static readonly PapyrusKeyword String = DefineKeyword("String", KeywordAttribute.NativeType);

        // Constants
        public static readonly PapyrusKeyword False = DefineKeyword("False", KeywordAttribute.CompileTimeConstant);
        public static readonly PapyrusKeyword True = DefineKeyword("True", KeywordAttribute.CompileTimeConstant);
        public static readonly PapyrusKeyword None = DefineKeyword("None", KeywordAttribute.CompileTimeConstant);

        // Special identifiers
        public static readonly PapyrusKeyword Self = DefineKeyword("Self", KeywordAttribute.SpecialIdentifier);
        public static readonly PapyrusKeyword Parent = DefineKeyword("Parent", KeywordAttribute.SpecialIdentifier);

        // Special members
        public static readonly PapyrusKeyword Length = DefineKeyword("Length", KeywordAttribute.SpecialMember);

        // Special operators
        public static readonly PapyrusKeyword As = DefineKeyword("As", KeywordAttribute.SpecialOperator);
        public static readonly PapyrusKeyword New = DefineKeyword("New", KeywordAttribute.SpecialOperator);
        public static readonly PapyrusKeyword Return = DefineKeyword("Return", KeywordAttribute.SpecialOperator);

        // Semantics
        public static readonly PapyrusKeyword If = DefineKeyword("If", KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword Else = DefineKeyword("Else", KeywordAttribute.IndentDecrease, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndIf = DefineKeyword("EndIf", KeywordAttribute.IndentDecrease);
        public static readonly PapyrusKeyword While = DefineKeyword("While", KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndWhile = DefineKeyword("EndWhile", KeywordAttribute.IndentDecrease);

        // Scopes
        public static readonly PapyrusKeyword Property = DefineKeyword("Property", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndProperty = DefineKeyword("EndProperty", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);
        public static readonly PapyrusKeyword Function = DefineKeyword("Function", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndFunction = DefineKeyword("EndFunction", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);
        public static readonly PapyrusKeyword Event = DefineKeyword("Event", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndEvent = DefineKeyword("EndEvent", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);
        public static readonly PapyrusKeyword State = DefineKeyword("State", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndState = DefineKeyword("EndState", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);

        #endregion
        #region Fallout 4 Keyword Definitions

        // Definition
        public static readonly PapyrusKeyword CustomEvent = DefineKeyword("CustomEvent");

        // Attributes
        public static readonly PapyrusKeyword DebugOnly = DefineKeyword("DebugOnly", KeywordAttribute.FunctionAttribute);
        public static readonly PapyrusKeyword BetaOnly = DefineKeyword("BetaOnly", KeywordAttribute.FunctionAttribute);
        public static readonly PapyrusKeyword Const = DefineKeyword("Const", KeywordAttribute.PropertyAttribute, KeywordAttribute.FieldAttribute);
        public static readonly PapyrusKeyword Mandatory = DefineKeyword("Mandatory", KeywordAttribute.PropertyAttribute);

        // Native types
        public static readonly PapyrusKeyword Var = DefineKeyword("Var", KeywordAttribute.NativeType);
        public static readonly PapyrusKeyword ScriptObject = DefineKeyword("ScriptObject", KeywordAttribute.NativeType);

        // Special operators
        public static readonly PapyrusKeyword Is = DefineKeyword("Is", KeywordAttribute.SpecialOperator);

        // Scopes
        public static readonly PapyrusKeyword Group = DefineKeyword("Group", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndGroup = DefineKeyword("EndGroup", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);
        public static readonly PapyrusKeyword Struct = DefineKeyword("Struct", KeywordAttribute.OutlineableBegin, KeywordAttribute.IndentIncreaseAtNewLine);
        public static readonly PapyrusKeyword EndStruct = DefineKeyword("EndStruct", KeywordAttribute.OutlineableEnd, KeywordAttribute.IndentDecrease);

        #endregion

        public PapyrusTokenType Type { get { return PapyrusTokenType.Keyword; } }
        public string Name { get; private set; }
        public int TokenSize { get { return Name.Length; } }
        public bool IsCompileTimeConstant { get { return attributes.Contains(KeywordAttribute.CompileTimeConstant); } }
        public bool IsLineExtension { get { return false; } }
        public bool IsIgnoredByParser { get { return false; } }
        private HashSet<KeywordAttribute> attributes;
        public IReadOnlyCollection<KeywordAttribute> Attributes { get { return attributes; } }

        private PapyrusKeyword(string name, KeywordAttribute[] attributes) {
            this.Name = name;
            this.attributes = new HashSet<KeywordAttribute>(attributes);
        }
        private static PapyrusKeyword DefineKeyword(string name, params KeywordAttribute[] attributes) {
            PapyrusKeyword keyword = new PapyrusKeyword(name, attributes);
            allKeywords.Add(name, keyword);
            return keyword;
        }

        IClassificationType ISyntaxColorableToken.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(PapyrusKeywordColorFormat.Name);
        }
    }

    internal sealed class PapyrusKeywordParser : ITokenParser {
        public bool TryParse(TokenParsingContext context, out IPapyrusToken token) {
            if (context.Scanner.CurrentState == TokenScannerState.Default) {
                int nextDelim = PapyrusDelimiter.FindNext(context.Source, 0);
                PapyrusKeyword keyword = PapyrusKeyword.FromName(context.Source.TryRemove(nextDelim));
                if (keyword != null) {
                    token = keyword;
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
    internal sealed class PapyrusKeywordColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusKeyword";

        internal PapyrusKeywordColorFormat() {
            DisplayName = "Papyrus Keyword";
            ForegroundColor = Color.FromRgb(86, 156, 214);
            IsBold = true;
        }
    }

    [DebuggerStepThrough]
    internal static class PapyrusKeywordColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(PapyrusKeywordColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }
}
