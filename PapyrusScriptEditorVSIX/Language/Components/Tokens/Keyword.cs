using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace Papyrus.Language.Components.Tokens {
    [DebuggerStepThrough]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Name)]
    [Name(Name)]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class KeywordColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusKeyword";

        internal KeywordColorFormat() {
            DisplayName = "Papyrus Keyword";
            ForegroundColor = Color.FromRgb(86, 156, 214);
            IsBold = true;
        }
    }

    [DebuggerStepThrough]
    internal static class KeywordColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(KeywordColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [Flags]
    public enum KeywordAttribute : ushort {
        None                        = 0x0000,
        Attribute                   = 0x0001,
        NativeType                  = 0x0002,
        CompileTimeConstant         = 0x0004,
        SpecialIdentifier           = 0x0008,
        SpecialMember               = 0x0008,
        SpecialOperator             = 0x0010,
        OutlineableBegin            = 0x0020,
        OutlineableEnd              = 0x0040,
        IndentIncreaseAtNewLine     = 0x0080,
        IndentDecrease              = 0x0100,
        BreakOutlineableProperty    = 0x0200,
        BreakOutlineableFunction    = 0x0400,
        PropertyTypeAttribute       = 0x0800,
        All                         = 0xffff,
    }

    public enum KeywordID : ushort {
        Scriptname,
        Extends,
        Import,

        Hidden,
        Conditional,
        Global,
        Auto,
        AutoReadOnly,
        Native,

        Bool,
        Int,
        Float,
        String,

        False,
        True,
        None,

        Self,
        Parent,

        Length,

        As,
        New,
        Return,

        If,
        Else,
        EndIf,
        While,
        EndWhile,

        Property,
        EndProperty,
        Function,
        EndFunction,
        Event,
        EndEvent,
        State,
        EndState,

        // FO4
        DebugOnly,
        BetaOnly,
        Const,
        Mandatory,
        Var,
        ScriptObject,
        Is,
        Struct,
        EndStruct,
        Group,
        EndGroup,
    }

    public sealed class Keyword : Token, ISyntaxColorable {
        #region Definitions

        private static readonly Dictionary<KeywordID, Keyword> allKeywords = new Dictionary<KeywordID, Keyword>() {
            { KeywordID.Scriptname, new Keyword(KeywordID.Scriptname) },
            { KeywordID.Extends, new Keyword(KeywordID.Extends) },
            { KeywordID.Import, new Keyword(KeywordID.Import) },

            { KeywordID.Hidden, new Keyword(KeywordID.Hidden, KeywordAttribute.Attribute) },
            { KeywordID.Conditional, new Keyword(KeywordID.Conditional, KeywordAttribute.Attribute | KeywordAttribute.Attribute) },
            { KeywordID.Global, new Keyword(KeywordID.Global, KeywordAttribute.Attribute) },
            { KeywordID.Auto, new Keyword(KeywordID.Auto, KeywordAttribute.PropertyTypeAttribute | KeywordAttribute.BreakOutlineableProperty) },
            { KeywordID.AutoReadOnly, new Keyword(KeywordID.AutoReadOnly, KeywordAttribute.PropertyTypeAttribute | KeywordAttribute.BreakOutlineableProperty) },
            { KeywordID.Native, new Keyword(KeywordID.Native, KeywordAttribute.Attribute | KeywordAttribute.BreakOutlineableFunction) },

            { KeywordID.Bool, new Keyword(KeywordID.Bool, KeywordAttribute.NativeType) },
            { KeywordID.Int, new Keyword(KeywordID.Int, KeywordAttribute.NativeType) },
            { KeywordID.Float, new Keyword(KeywordID.Float, KeywordAttribute.NativeType) },
            { KeywordID.String, new Keyword(KeywordID.String, KeywordAttribute.NativeType) },

            { KeywordID.False, new Keyword(KeywordID.False, KeywordAttribute.CompileTimeConstant) },
            { KeywordID.True, new Keyword(KeywordID.True, KeywordAttribute.CompileTimeConstant) },
            { KeywordID.None, new Keyword(KeywordID.None, KeywordAttribute.CompileTimeConstant) },

            { KeywordID.Self, new Keyword(KeywordID.Self, KeywordAttribute.SpecialIdentifier) },
            { KeywordID.Parent, new Keyword(KeywordID.Parent, KeywordAttribute.SpecialIdentifier) },

            { KeywordID.Length, new Keyword(KeywordID.Length, KeywordAttribute.SpecialMember) },

            { KeywordID.As, new Keyword(KeywordID.As, KeywordAttribute.SpecialOperator) },
            { KeywordID.New, new Keyword(KeywordID.New, KeywordAttribute.SpecialOperator) },
            { KeywordID.Return, new Keyword(KeywordID.Return, KeywordAttribute.SpecialOperator) },

            { KeywordID.If, new Keyword(KeywordID.If, KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.Else, new Keyword(KeywordID.Else, KeywordAttribute.IndentDecrease | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndIf, new Keyword(KeywordID.EndIf, KeywordAttribute.IndentDecrease) },
            { KeywordID.While, new Keyword(KeywordID.While, KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndWhile, new Keyword(KeywordID.EndWhile, KeywordAttribute.IndentDecrease) },

            { KeywordID.Property, new Keyword(KeywordID.Property, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndProperty, new Keyword(KeywordID.EndProperty, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease) },
            { KeywordID.Function, new Keyword(KeywordID.Function, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndFunction, new Keyword(KeywordID.EndFunction, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease) },
            { KeywordID.Event, new Keyword(KeywordID.Event, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndEvent, new Keyword(KeywordID.EndEvent, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease) },
            { KeywordID.State, new Keyword(KeywordID.State, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndState, new Keyword(KeywordID.EndState, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease) },

            // FO4
            { KeywordID.DebugOnly, new Keyword(KeywordID.DebugOnly, KeywordAttribute.Attribute) },
            { KeywordID.BetaOnly, new Keyword(KeywordID.BetaOnly, KeywordAttribute.Attribute) },
            { KeywordID.Const, new Keyword(KeywordID.Const, KeywordAttribute.Attribute) },
            { KeywordID.Mandatory, new Keyword(KeywordID.Mandatory, KeywordAttribute.Attribute) },
            { KeywordID.Var, new Keyword(KeywordID.Var, KeywordAttribute.NativeType) },
            { KeywordID.ScriptObject, new Keyword(KeywordID.ScriptObject, KeywordAttribute.NativeType) },
            { KeywordID.Is, new Keyword(KeywordID.Is, KeywordAttribute.SpecialOperator) },
            { KeywordID.Struct, new Keyword(KeywordID.Struct, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine) },
            { KeywordID.EndStruct, new Keyword(KeywordID.EndStruct, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease) },
        };
        public static IReadOnlyCollection<Keyword> All {
            get { return allKeywords.Values; }
        }

        public static Keyword Scriptname { get { return allKeywords[KeywordID.Scriptname]; } }
        public static Keyword Extends { get { return allKeywords[KeywordID.Extends]; } }
        public static Keyword Import { get { return allKeywords[KeywordID.Import]; } }

        public static Keyword Hidden { get { return allKeywords[KeywordID.Hidden]; } }
        public static Keyword Conditional { get { return allKeywords[KeywordID.Conditional]; } }
        public static Keyword Global { get { return allKeywords[KeywordID.Global]; } }
        public static Keyword Auto { get { return allKeywords[KeywordID.Auto]; } }
        public static Keyword AutoReadOnly { get { return allKeywords[KeywordID.AutoReadOnly]; } }
        public static Keyword Native { get { return allKeywords[KeywordID.Native]; } }

        public static Keyword Bool { get { return allKeywords[KeywordID.Bool]; } }
        public static Keyword Int { get { return allKeywords[KeywordID.Int]; } }
        public static Keyword Float { get { return allKeywords[KeywordID.Float]; } }
        public static Keyword String { get { return allKeywords[KeywordID.String]; } }

        public static Keyword False { get { return allKeywords[KeywordID.False]; } }
        public static Keyword True { get { return allKeywords[KeywordID.True]; } }
        public static Keyword None { get { return allKeywords[KeywordID.None]; } }

        public static Keyword Self { get { return allKeywords[KeywordID.Self]; } }
        public static Keyword Parent { get { return allKeywords[KeywordID.Parent]; } }

        public static Keyword Length { get { return allKeywords[KeywordID.Length]; } }

        public static Keyword As { get { return allKeywords[KeywordID.As]; } }
        public static Keyword New { get { return allKeywords[KeywordID.New]; } }
        public static Keyword Return { get { return allKeywords[KeywordID.Return]; } }

        public static Keyword If { get { return allKeywords[KeywordID.If]; } }
        public static Keyword Else { get { return allKeywords[KeywordID.Else]; } }
        public static Keyword EndIf { get { return allKeywords[KeywordID.EndIf]; } }
        public static Keyword While { get { return allKeywords[KeywordID.While]; } }
        public static Keyword EndWhile { get { return allKeywords[KeywordID.EndWhile]; } }

        public static Keyword Property { get { return allKeywords[KeywordID.Property]; } }
        public static Keyword EndProperty { get { return allKeywords[KeywordID.EndProperty]; } }
        public static Keyword Function { get { return allKeywords[KeywordID.Function]; } }
        public static Keyword EndFunction { get { return allKeywords[KeywordID.EndFunction]; } }
        public static Keyword Event { get { return allKeywords[KeywordID.Event]; } }
        public static Keyword EndEvent { get { return allKeywords[KeywordID.EndEvent]; } }
        public static Keyword State { get { return allKeywords[KeywordID.State]; } }
        public static Keyword EndState { get { return allKeywords[KeywordID.EndState]; } }

        // FO4
        public static Keyword DebugOnly { get { return allKeywords[KeywordID.DebugOnly]; } }
        public static Keyword BetaOnly { get { return allKeywords[KeywordID.BetaOnly]; } }
        public static Keyword Const { get { return allKeywords[KeywordID.Const]; } }
        public static Keyword Mandatory { get { return allKeywords[KeywordID.Mandatory]; } }
        public static Keyword Var { get { return allKeywords[KeywordID.Var]; } }
        public static Keyword ScriptObject { get { return allKeywords[KeywordID.ScriptObject]; } }
        public static Keyword Is { get { return allKeywords[KeywordID.Is]; } }
        public static Keyword Struct { get { return allKeywords[KeywordID.Struct]; } }
        public static Keyword EndStruct { get { return allKeywords[KeywordID.EndStruct]; } }

        #endregion

        private KeywordID id;
        private KeywordAttribute attributes;
        private IndentBehavior indentBehavior;

        //[DebuggerStepThrough]
        private Keyword(KeywordID id, KeywordAttribute attributes, IndentBehavior indentBehavior) {
            this.id = id;
            this.attributes = attributes;
            this.indentBehavior = indentBehavior;
        }
        //[DebuggerStepThrough]
        private Keyword(KeywordID id, KeywordAttribute attributes) :
            this(id, attributes, IndentBehavior.DoNothing) {
        }
        //[DebuggerStepThrough]
        private Keyword(KeywordID id, IndentBehavior indentBehavior) :
            this(id, KeywordAttribute.None, indentBehavior) {
        }
        //[DebuggerStepThrough]
        private Keyword(KeywordID id) :
            this(id, KeywordAttribute.None, IndentBehavior.DoNothing) {
        }

        public KeywordAttribute Attributes {
            get { return attributes; }
        }

        public override string Text {
            get { return id.ToString(); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Keyword; }
        }

        public override IndentBehavior IndentBehavior {
            get { return indentBehavior; }
        }

        public override bool IsOutlineableStart(IReadOnlyTokenSnapshotLine line) {
            if (attributes.HasFlag(KeywordAttribute.OutlineableBegin)) {
                switch (id) {
                    case KeywordID.Property:
                        return line.All(t => {
                            return t.Type.TypeID != TokenTypeID.Keyword || ((Keyword)t.Type).attributes.HasFlag(KeywordAttribute.BreakOutlineableProperty) == false;
                        });
                    case KeywordID.Function:
                        return line.All(t => {
                            return t.Type.TypeID != TokenTypeID.Keyword || ((Keyword)t.Type).attributes.HasFlag(KeywordAttribute.BreakOutlineableFunction) == false;
                        });
                    default:
                        return true;
                }
            }
            return false;
        }
        public override bool IsOutlineableEnd(Token startToken) {
            if (this.attributes.HasFlag(KeywordAttribute.OutlineableEnd)) {
                return startToken is Keyword && ((Keyword)startToken).attributes.HasFlag(KeywordAttribute.OutlineableBegin) &&
                    System.String.Equals(this.ToString(), System.String.Concat("End", startToken.Text), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override bool IsCompileTimeConstant {
            get { return attributes.HasFlag(KeywordAttribute.CompileTimeConstant); }
        }

        public override bool IsVariableType {
            get { return attributes.HasFlag(KeywordAttribute.NativeType); }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(KeywordColorFormat.Name);
        }

        public override int GetHashCode() {
            return id.GetHashCode();
        }
        public override bool Equals(object obj) {
            return obj is Keyword && this.id == ((Keyword)obj).id;
        }

        public static implicit operator KeywordID(Keyword keyword) {
            return keyword.id;
        }
        public static explicit operator string(Keyword keyword) {
            return keyword.Text;
        }

        public static bool operator ==(Keyword x, Keyword y) {
            return Equals(x, y);
        }
        public static bool operator !=(Keyword x, Keyword y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Keyword x, Token y) {
            return x == y as Keyword;
        }
        public static bool operator !=(Keyword x, Token y) {
            return x != y as Keyword;
        }
        public static bool operator ==(Token x, Keyword y) {
            return x as Keyword == y;
        }
        public static bool operator !=(Token x, Keyword y) {
            return x as Keyword != y;
        }

        public static Keyword Parse(string token) {
            if (token.All(c => Char.IsLetter(c))) {
                KeywordID id;
                Keyword keyword;
                if (Enum.TryParse(token, true, out id) &&
                    allKeywords.TryGetValue(id, out keyword)) {
                    return keyword;
                }
            }
            return null;
        }
    }

    internal sealed class KeywordParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo token) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                Keyword keyword = Keyword.Parse(text, 0, Delimiter.FindNext(text, 0));
                if (keyword != null) {
                    token.Type = keyword;
                    token.Span = sourceSnapshotSpan.Subspan(0, keyword.Text.Length);
                    return true;
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, out Token token) {
            if (state == TokenScannerState.Text) {
                int nextDelim = Delimiter.FindNext(sourceTextSpan, 0);
                Keyword keyword = Keyword.Parse(nextDelim == sourceTextSpan.Length ? sourceTextSpan : sourceTextSpan.Remove(nextDelim));
                if (keyword != null) {
                    token = keyword;
                    return true;
                }
            }
            token = null;
            return false;
        }
    }
}
