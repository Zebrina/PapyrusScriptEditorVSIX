using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
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

    /*
     * 
     * { KeywordID.Scriptname, new Keyword(KeywordID.Scriptname) },
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
     * 
     * */

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
    
    public sealed class Keyword : Token, ISyntaxColorable, IOutlineableToken {
        #region Definitions

        [StaticToken]
        public static readonly Keyword Scriptname = new Keyword(KeywordID.Scriptname);
        [StaticToken]
        public static readonly Keyword Extends = new Keyword(KeywordID.Extends);
        [StaticToken]
        public static readonly Keyword Import = new Keyword(KeywordID.Import);

        [StaticToken]
        public static readonly Keyword Hidden = new Keyword(KeywordID.Hidden, KeywordAttribute.Attribute);
        [StaticToken]
        public static readonly Keyword Conditional = new Keyword(KeywordID.Conditional, KeywordAttribute.Attribute | KeywordAttribute.Attribute);
        [StaticToken]
        public static readonly Keyword Global = new Keyword(KeywordID.Global, KeywordAttribute.Attribute);
        [StaticToken]
        public static readonly Keyword Auto = new Keyword(KeywordID.Auto, KeywordAttribute.PropertyTypeAttribute | KeywordAttribute.BreakOutlineableProperty);
        [StaticToken]
        public static readonly Keyword AutoReadOnly = new Keyword(KeywordID.AutoReadOnly, KeywordAttribute.PropertyTypeAttribute | KeywordAttribute.BreakOutlineableProperty);
        [StaticToken]
        public static readonly Keyword Native = new Keyword(KeywordID.Native, KeywordAttribute.Attribute | KeywordAttribute.BreakOutlineableFunction);

        [StaticToken]
        public static readonly Keyword Bool = new Keyword(KeywordID.Bool, KeywordAttribute.NativeType);
        [StaticToken]
        public static readonly Keyword Int = new Keyword(KeywordID.Int, KeywordAttribute.NativeType);
        [StaticToken]
        public static readonly Keyword Float = new Keyword(KeywordID.Float, KeywordAttribute.NativeType);
        [StaticToken]
        public static readonly Keyword String = new Keyword(KeywordID.String, KeywordAttribute.NativeType);

        [StaticToken]
        public static readonly Keyword False = new Keyword(KeywordID.False, KeywordAttribute.CompileTimeConstant);
        [StaticToken]
        public static readonly Keyword True = new Keyword(KeywordID.True, KeywordAttribute.CompileTimeConstant);
        [StaticToken]
        public static readonly Keyword None = new Keyword(KeywordID.None, KeywordAttribute.CompileTimeConstant);

        [StaticToken]
        public static readonly Keyword Self = new Keyword(KeywordID.Self, KeywordAttribute.SpecialIdentifier);
        [StaticToken]
        public static readonly Keyword Parent = new Keyword(KeywordID.Parent, KeywordAttribute.SpecialIdentifier);

        [StaticToken]
        public static readonly Keyword Length = new Keyword(KeywordID.Length, KeywordAttribute.SpecialMember);

        [StaticToken]
        public static readonly Keyword As = new Keyword(KeywordID.As, KeywordAttribute.SpecialOperator);
        [StaticToken]
        public static readonly Keyword New = new Keyword(KeywordID.New, KeywordAttribute.SpecialOperator);
        [StaticToken]
        public static readonly Keyword Return = new Keyword(KeywordID.Return, KeywordAttribute.SpecialOperator);

        [StaticToken]
        public static readonly Keyword If = new Keyword(KeywordID.If, KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword Else = new Keyword(KeywordID.Else, KeywordAttribute.IndentDecrease | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndIf = new Keyword(KeywordID.EndIf, KeywordAttribute.IndentDecrease);
        [StaticToken]
        public static readonly Keyword While = new Keyword(KeywordID.While, KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndWhile = new Keyword(KeywordID.EndWhile, KeywordAttribute.IndentDecrease);

        [StaticToken]
        public static readonly Keyword Property = new Keyword(KeywordID.Property, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndProperty = new Keyword(KeywordID.EndProperty, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);
        [StaticToken]
        public static readonly Keyword Function = new Keyword(KeywordID.Function, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndFunction = new Keyword(KeywordID.EndFunction, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);
        [StaticToken]
        public static readonly Keyword Event = new Keyword(KeywordID.Event, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndEvent = new Keyword(KeywordID.EndEvent, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);
        [StaticToken]
        public static readonly Keyword State = new Keyword(KeywordID.State, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken]
        public static readonly Keyword EndState = new Keyword(KeywordID.EndState, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);

        // FO4
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword DebugOnly = new Keyword(KeywordID.DebugOnly, KeywordAttribute.Attribute);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword BetaOnly = new Keyword(KeywordID.BetaOnly, KeywordAttribute.Attribute);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Const = new Keyword(KeywordID.Const, KeywordAttribute.Attribute);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Mandatory = new Keyword(KeywordID.Mandatory, KeywordAttribute.Attribute);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Var = new Keyword(KeywordID.Var, KeywordAttribute.NativeType);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword ScriptObject = new Keyword(KeywordID.ScriptObject, KeywordAttribute.NativeType);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Is = new Keyword(KeywordID.Is, KeywordAttribute.SpecialOperator);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Struct = new Keyword(KeywordID.Struct, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword EndStruct = new Keyword(KeywordID.EndStruct, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword Group = new Keyword(KeywordID.Group, KeywordAttribute.OutlineableBegin | KeywordAttribute.IndentIncreaseAtNewLine);
        [StaticToken(typeof(FO4GameInfo))]
        public static readonly Keyword EndGroup = new Keyword(KeywordID.EndGroup, KeywordAttribute.OutlineableEnd | KeywordAttribute.IndentDecrease);

        private static TokenManager<Keyword> manager = new TokenManager<Keyword>(true);
        public static TokenManager<Keyword> Manager {
            get { return manager; }
        }

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

        bool IOutlineableToken.IsOutlineableStart(IReadOnlyTokenSnapshotLine line) {
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
        bool IOutlineableToken.IsOutlineableEnd(IOutlineableToken startToken) {
            if (this.attributes.HasFlag(KeywordAttribute.OutlineableEnd)) {
                return startToken is Keyword && ((Keyword)startToken).attributes.HasFlag(KeywordAttribute.OutlineableBegin) &&
                    System.String.Equals(this.ToString(), System.String.Concat("End", startToken.ToString()), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        bool IOutlineableToken.IsImplementation {
            get { return true; }
        }
        string IOutlineableToken.CollapsedText {
            get { return "..."; }
        }
        bool IOutlineableToken.CollapseFirstLine {
            get { return false; }
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
            return Hash.GetMemberwiseHashCode(id, attributes, indentBehavior);
        }
        public override bool Equals(object obj) {
            Keyword objAsKeyword = obj as Keyword;
            return objAsKeyword != null &&
                this.id == objAsKeyword.id &&
                this.attributes == objAsKeyword.attributes &&
                this.indentBehavior == objAsKeyword.indentBehavior;
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
            return manager.ParseToken(token);

            /*
            KeywordID id;
            if (Enum.TryParse(token, true, out id)) {
                return allKeywords[id];
            }
            return null;
            */
        }
    }

    internal sealed class KeywordParser : TokenParser {
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token) {
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
