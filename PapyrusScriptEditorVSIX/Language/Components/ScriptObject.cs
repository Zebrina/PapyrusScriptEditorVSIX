using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections;
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
    internal sealed class ScriptObjectColorFormat : ClassificationFormatDefinition {
        internal const string Name = "PapyrusScriptObject";

        internal ScriptObjectColorFormat() {
            DisplayName = "Papyrus Script Object";
            ForegroundColor = Color.FromRgb(78, 201, 176);
            IsBold = true;
        }
    }

    internal static class ScriptObjectColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ScriptObjectColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

    [DebuggerStepThrough]
    public struct ScriptInfo : IDisposable {
        public ScriptInfo(string name, ScriptObject parent, bool hidden, bool conditional) {
            this.Name = name;
            this.Parent = parent;
            this.Conditional = conditional;
            this.Hidden = hidden;
        }
        public ScriptInfo(string name, bool hidden, bool conditional) :
            this(name, null, hidden, conditional) {
        }
        public ScriptInfo(string name) :
            this(name, null, false, false) {
        }

        public string Name { get; set; }
        public ScriptObject Parent { get; set; } // Null if no parent.
        public bool Conditional { get; set; }
        public bool Hidden { get; set; }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj) {
            if (obj.GetType() == this.GetType()) {
                ScriptInfo other = (ScriptInfo)obj;
                return this.Name == other.Name &&
                    this.Parent == other.Parent &&
                    this.Conditional == other.Conditional &&
                    this.Hidden == other.Hidden;
            }
            return false;
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            b.Append(Keyword.Scriptname);
            b.Append(' ');
            b.Append(Name);

            if (Parent != null) {
                b.Append(' ');
                b.Append(Keyword.Extends);
                b.Append(' ');
                b.Append(Parent.Text);
            }

            if (Conditional) {
                b.Append(' ');
                b.Append(Keyword.Conditional);
            }
            if (Hidden) {
                b.Append(' ');
                b.Append(Keyword.Hidden);
            }

            return b.ToString();
        }

        public void Dispose() {
            Name = null;
            Parent = null;
        }

        public static bool operator ==(ScriptInfo x, ScriptInfo y) {
            return x.Equals(y);
        }
        public static bool operator !=(ScriptInfo x, ScriptInfo y) {
            return !x.Equals(y);
        }
    }

    public sealed class ScriptObject : TokenType, ISyntaxColorable, ICollection<IScriptMember>, IEnumerable<IScriptMember>, IDisposable {
        private class ScriptMemberComparer : IComparer<IScriptMember> {
            public int Compare(IScriptMember x, IScriptMember y) {
                return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        private SortedSet<IScriptMember> members;

        public ScriptInfo Info { get; set; }
        public IReadOnlyCollection<IScriptMember> Members {
            get { return members; }
        }
        public bool Disposed { get; private set; }

        [DebuggerStepThrough]
        public ScriptObject(ScriptInfo info) {
            this.Info = info;
            this.members = new SortedSet<IScriptMember>(new ScriptMemberComparer());
        }

        public override string Text {
            get { return Info.Name; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.ScriptObject; }
        }
        public override TokenColorID Color {
            get { return TokenColorID.ScriptObject; }
        }

        public override bool VariableType {
            get { return true; }
        }

        IClassificationType ISyntaxColorable.GetClassificationType(IClassificationTypeRegistryService registry) {
            return registry.GetClassificationType(ScriptObjectColorFormat.Name);
        }

        public int Count {
            get { return members.Count; }
        }
        public bool IsReadOnly {
            get { return ((ICollection<IScriptMember>)members).IsReadOnly; }
        }

        public void Add(IScriptMember newMember) {
            if (newMember != null) {
                members.Add(newMember);
            }
        }
        
        public override IReadOnlyCollection<IScriptMember> GetMemberList() {
            SortedSet<IScriptMember> generatedMemberList = new SortedSet<IScriptMember>(members, new ScriptMemberComparer());
            if (Info.Parent != null) {
                // Union with accessible parent members.
                generatedMemberList.UnionWith(Info.Parent.GetMemberList().Where(m => { return m.ChildAccessible; }));
            }
            return generatedMemberList;
        }

        public bool Contains(IScriptMember item) {
            return ((ICollection<IScriptMember>)members).Contains(item);
        }

        public void CopyTo(IScriptMember[] array, int arrayIndex) {
            ((ICollection<IScriptMember>)members).CopyTo(array, arrayIndex);
        }

        public bool Remove(IScriptMember item) {
            return ((ICollection<IScriptMember>)members).Remove(item);
        }
        public void Clear() {
            members.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return members.GetEnumerator();
        }
        public IEnumerator<IScriptMember> GetEnumerator() {
            return members.GetEnumerator();
        }

        public void Dispose() {
            if (!Disposed) {
                Info.Dispose();
                members = null;
                Disposed = true;
            }
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            b.Append(Info);
            b.AppendLine();

            foreach (IScriptMember member in GetMemberList()) {
                b.Append('\t');
                b.Append(member);
                b.AppendLine();
            }

            b.AppendLine();

            return b.ToString();
        }

        public static ScriptObject TryParse(string name) {
            return ScriptObjectManager.Instance.Get(name);
        }
        public static bool TryParse(string name, out ScriptObject scriptObject) {
            scriptObject = ScriptObjectManager.Instance.Get(name);
            return scriptObject != null;
        }

        
    }

    public class ScriptObjectManager : Singleton<ScriptObjectManager>, ICollection<ScriptObject>, IReadOnlyDictionary<string, ScriptObject> {
        private SortedList<string, ScriptObject> scriptObjectList = new SortedList<string, ScriptObject>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> Keys { get { return scriptObjectList.Keys; } }
        public IEnumerable<ScriptObject> Values { get { return scriptObjectList.Values; } }
        public int Count { get { return scriptObjectList.Count; } }

        public bool IsReadOnly {
            get { return scriptObjectList.Values.IsReadOnly; }
        }

        public ScriptObject this[string key] {
            get { return Get(key); }
        }

        public ScriptObject Get(string scriptName) {
            ScriptObject value;
            if (scriptObjectList.TryGetValue(scriptName, out value)) {
                return value;
            }

            return null;
        }

        public void Add(ScriptObject item) {
            scriptObjectList.Add(item.Text, item);
        }

        public bool TryGetValue(string key, out ScriptObject value) {
            return this.scriptObjectList.TryGetValue(key, out value);
        }

        public bool Remove(ScriptObject item) {
            if (item != null) {
                string key = item.Text;
                if (scriptObjectList.TryGetValue(key, out item)) {
                    item.Dispose();
                    return scriptObjectList.Remove(key);
                }
            }
            return false;
        }
        public bool RemoveByKey(string key) {
            ScriptObject item;
            if (scriptObjectList.TryGetValue(key, out item)) {
                item.Dispose();
                return scriptObjectList.Remove(key);
            }
            return false;
        }

        public bool Contains(ScriptObject item) {
            return item != null && scriptObjectList.ContainsKey(item.Text);
        }
        public bool ContainsKey(string key) {
            return scriptObjectList.ContainsKey(key);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return scriptObjectList.GetEnumerator();
        }
        IEnumerator<ScriptObject> IEnumerable<ScriptObject>.GetEnumerator() {
            return scriptObjectList.Values.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<string, ScriptObject>> GetEnumerator() {
            return scriptObjectList.GetEnumerator();
        }

        public void CopyTo(ScriptObject[] array, int arrayIndex) {
            scriptObjectList.Values.CopyTo(array, arrayIndex);
        }

        public void Clear() {
            scriptObjectList.Clear();
        }
    }

    /*
    [DebuggerStepThrough]
    public class FieldScriptMember : IScriptMember {
        public Token VariableType { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }

        public FieldScriptMember(Token variableType, string name, string defaultValue = "") {
            this.VariableType = variableType;
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
        
        public Token Type { get { return VariableType; } }
        public string MemberName { get { return Name; } }
        public bool ChildAccessible { get { return true; } }
        public IReadOnlyList<Parameter> Parameters { get { return null; } }

        public int CompareTo(IScriptMember obj) {
            return Name.CompareTo(obj.Name);
        }
    }
    */
}
