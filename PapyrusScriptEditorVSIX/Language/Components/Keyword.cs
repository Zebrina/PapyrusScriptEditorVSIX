using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Features;
using Papyrus.Language.Parsing;
using System;
using System.Collections.Generic;
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
    internal sealed class KeywordColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusKeyword";

        internal KeywordColorFormat() {
            DisplayName = "Papyrus Keyword";
            ForegroundColor = Color.FromRgb(86, 156, 214);
            IsBold = true;
        }
    }

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
    }

    public class Keyword : TokenType, ISyntaxColorable {
        #region Keyword Definitions

        private static readonly Dictionary<KeywordID, Keyword> collection = new Dictionary<KeywordID, Keyword>() {
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

        public static Keyword Scriptname { get { return collection[KeywordID.Scriptname]; } }
        public static Keyword Extends { get { return collection[KeywordID.Extends]; } }
        public static Keyword Import { get { return collection[KeywordID.Import]; } }

        public static Keyword Hidden { get { return collection[KeywordID.Hidden]; } }
        public static Keyword Conditional { get { return collection[KeywordID.Conditional]; } }
        public static Keyword Global { get { return collection[KeywordID.Global]; } }
        public static Keyword Auto { get { return collection[KeywordID.Auto]; } }
        public static Keyword AutoReadOnly { get { return collection[KeywordID.AutoReadOnly]; } }
        public static Keyword Native { get { return collection[KeywordID.Native]; } }

        public static Keyword Bool { get { return collection[KeywordID.Bool]; } }
        public static Keyword Int { get { return collection[KeywordID.Int]; } }
        public static Keyword Float { get { return collection[KeywordID.Float]; } }
        public static Keyword String { get { return collection[KeywordID.String]; } }

        public static Keyword False { get { return collection[KeywordID.False]; } }
        public static Keyword True { get { return collection[KeywordID.True]; } }
        public static Keyword None { get { return collection[KeywordID.None]; } }

        public static Keyword Self { get { return collection[KeywordID.Self]; } }
        public static Keyword Parent { get { return collection[KeywordID.Parent]; } }

        public static Keyword Length { get { return collection[KeywordID.Length]; } }

        public static Keyword As { get { return collection[KeywordID.As]; } }
        public static Keyword New { get { return collection[KeywordID.New]; } }
        public static Keyword Return { get { return collection[KeywordID.Return]; } }

        public static Keyword If { get { return collection[KeywordID.If]; } }
        public static Keyword Else { get { return collection[KeywordID.Else]; } }
        public static Keyword EndIf { get { return collection[KeywordID.EndIf]; } }
        public static Keyword While { get { return collection[KeywordID.While]; } }
        public static Keyword EndWhile { get { return collection[KeywordID.EndWhile]; } }

        public static Keyword Property { get { return collection[KeywordID.Property]; } }
        public static Keyword EndProperty { get { return collection[KeywordID.EndProperty]; } }
        public static Keyword Function { get { return collection[KeywordID.Function]; } }
        public static Keyword EndFunction { get { return collection[KeywordID.EndFunction]; } }
        public static Keyword Event { get { return collection[KeywordID.Event]; } }
        public static Keyword EndEvent { get { return collection[KeywordID.EndEvent]; } }
        public static Keyword State { get { return collection[KeywordID.State]; } }
        public static Keyword EndState { get { return collection[KeywordID.EndState]; } }

        // FO4
        public static Keyword DebugOnly { get { return collection[KeywordID.DebugOnly]; } }
        public static Keyword BetaOnly { get { return collection[KeywordID.BetaOnly]; } }
        public static Keyword Const { get { return collection[KeywordID.Const]; } }
        public static Keyword Mandatory { get { return collection[KeywordID.Mandatory]; } }
        public static Keyword Var { get { return collection[KeywordID.Var]; } }
        public static Keyword ScriptObject { get { return collection[KeywordID.ScriptObject]; } }
        public static Keyword Is { get { return collection[KeywordID.Is]; } }
        public static Keyword Struct { get { return collection[KeywordID.Struct]; } }
        public static Keyword EndStruct { get { return collection[KeywordID.EndStruct]; } }

        #endregion

        private KeywordID id;
        private KeywordAttribute attributes;
        private IndentBehavior indentBehavior;

        [DebuggerStepThrough]
        private Keyword(KeywordID id, KeywordAttribute attributes, IndentBehavior indentBehavior) {
            this.id = id;
            this.attributes = attributes;
            this.indentBehavior = indentBehavior;
        }
        [DebuggerStepThrough]
        private Keyword(KeywordID id, KeywordAttribute attributes) :
            this(id, attributes, IndentBehavior.DoNothing) { }
        [DebuggerStepThrough]
        private Keyword(KeywordID id, IndentBehavior indentBehavior) :
            this(id, KeywordAttribute.None, indentBehavior) { }
        [DebuggerStepThrough]
        private Keyword(KeywordID id) :
            this(id, KeywordAttribute.None, IndentBehavior.DoNothing) { }

        public override string Text {
            get { return id.ToString(); }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.Keyword; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.Keyword; }
        }

        public override IndentBehavior IndentBehavior {
            get { return indentBehavior; }
        }

        public override bool IsOutlineableStart(IReadOnlyParsedLine line) {
            if (attributes.HasFlag(KeywordAttribute.OutlineableBegin)) {
                switch (id) {
                    case KeywordID.Property:
                        return !line.Any(t => {
                            return t.TypeID == TokenTypeID.Keyword && ((Keyword)t).attributes.HasFlag(KeywordAttribute.BreakOutlineableProperty);
                        });
                    case KeywordID.Function:
                        return !line.Any(t => {
                            return t.TypeID == TokenTypeID.Keyword && ((Keyword)t).attributes.HasFlag(KeywordAttribute.BreakOutlineableFunction);
                        });
                    default:
                        return true;
                }
            }
            return false;
        }
        public override bool IsOutlineableEnd(TokenType startToken) {
            return startToken is Keyword && this.attributes.HasFlag(KeywordAttribute.OutlineableBegin) &&
                System.String.Equals(this.ToString(), "End" + startToken.Text, StringComparison.OrdinalIgnoreCase);
        }

        public override bool CompileTimeConstant {
            get { return attributes.HasFlag(KeywordAttribute.CompileTimeConstant); }
        }

        public override bool VariableType {
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

        public static implicit operator string(Keyword keyword) {
            return keyword.Text;
        }

        public static bool operator ==(Keyword x, Keyword y) {
            return Equals(x, y);
        }
        public static bool operator !=(Keyword x, Keyword y) {
            return !Equals(x, y);
        }
        public static bool operator ==(Keyword x, TokenType y) {
            return x == (Keyword)y;
        }
        public static bool operator !=(Keyword x, TokenType y) {
            return x != (Keyword)y;
        }

        public static Keyword Parse(string source, int offset, int length) {
            KeywordID id;
            Keyword keyword;
            if (Enum.TryParse(source.Substring(offset, length), true, out id) &&
                collection.TryGetValue(id, out keyword)) {
                return keyword;
            }
            return null;
        }
    } 
}
