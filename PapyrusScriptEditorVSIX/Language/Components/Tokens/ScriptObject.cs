using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Papyrus.Common;
using Papyrus.Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Papyrus.Language.Components.Tokens {
    [DebuggerStepThrough]
    internal static class ScriptObjectColorClassificationDefinition {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ScriptObjectColorFormat.Name)]
        private static ClassificationTypeDefinition typeDefinition;
    }

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

    //[DebuggerStepThrough]
    public struct ScriptInfo : ISyntaxParsable {
        public ScriptInfo(string name, ScriptObject parent, bool conditional, bool hidden) {
            this.Name = name;
            this.parent = parent;
            this.Attributes = new ScriptMemberAttributes();
            if (hidden) this.Attributes.Add(Keyword.Conditional);
            if (hidden) this.Attributes.Add(Keyword.Hidden);
        }
        public ScriptInfo(string name, bool hidden, bool conditional) :
            this(name, null, hidden, conditional) {
        }
        public ScriptInfo(string name) :
            this(name, null, false, false) {
        }

        private ScriptObject parent;

        public string Name { get; set; }
        public ScriptObject Parent {
            get { return parent; }
            set { parent = null; }
        }
        public ScriptMemberAttributes Attributes { get; private set; }

        public bool IsValid {
            get { return Name != null; }
        }

        public int Length {
            get { return 2 + (Parent != null ? 2 : 0) + Attributes.Length; }
        }

        public bool TryParse(IReadOnlyList<Token> tokens, int offset) {
            int remainingTokenCount = tokens.Count - offset;
            if (remainingTokenCount >= 2 && tokens[offset] == Keyword.Scriptname && tokens[offset + 1].TypeID == TokenTypeID.ScriptObject) {
                Name = tokens[offset + 1].Text;

                offset += 2;
                if (remainingTokenCount >= 4 && tokens[offset] == Keyword.Scriptname && tokens[offset + 1].TypeID == TokenTypeID.ScriptObject) {
                    parent = ScriptObject.Manager.GetScriptObject(tokens[offset + 1].Text, true);
                    offset += 2;
                }

                Attributes.TryParse(tokens, offset);

                return true;
            }

            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj) {
            if (obj.GetType() == this.GetType()) {
                ScriptInfo other = (ScriptInfo)obj;
                return this.Name == other.Name &&
                    this.Parent == other.Parent &&
                    this.Attributes.Equals(other.Attributes);
            }
            return false;
        }

        public override string ToString() {
            StringBuilder b = new StringBuilder();

            b.Append(Keyword.Scriptname);
            b.AppendWhiteSpace();
            b.Append(Name);

            if (Parent != null) {
                b.AppendWhiteSpace();
                b.Append(Keyword.Extends);
                b.AppendWhiteSpace();
                b.Append(Parent.Text);
            }

            b.Append(Attributes);

            return b.ToString();
        }

        public static bool operator ==(ScriptInfo x, ScriptInfo y) {
            return x.Equals(y);
        }
        public static bool operator !=(ScriptInfo x, ScriptInfo y) {
            return !x.Equals(y);
        }
    }

    public sealed partial class ScriptObject : Token, ISyntaxColorable, ICollection<IScriptMember>, IEnumerable<IScriptMember>, ICloneable {
        private static readonly TokenManager<ScriptObject> manager = new TokenManager<ScriptObject>(false);

        private class ScriptMemberComparer : IComparer<IScriptMember> {
            public int Compare(IScriptMember x, IScriptMember y) {
                return String.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }
        }

        private readonly SortedSet<IScriptMember> members;

        public ScriptInfo Info { get; private set; }

        public ScriptObject(string name) {
            this.Info = new ScriptInfo(name);
        }
        public ScriptObject(ScriptObject other) {
            this.members = new SortedSet<IScriptMember>(other.members);
            this.Info = other.Info;
        }
        public ScriptObject() {
            Info = new ScriptInfo(String.Empty);
            members = new SortedSet<IScriptMember>();
        }

        public override string Text {
            get { return Info.Name; }
        }
        public override TokenTypeID TypeID {
            get { return TokenTypeID.ScriptObject; }
        }

        public override bool IsVariableType {
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
                generatedMemberList.UnionWith(Info.Parent.GetMemberList().Where(m => { return m.IsChildAccessible; }));
            }
            return generatedMemberList;
        }

        public bool Contains(IScriptMember item) {
            return members.Contains(item);
        }

        public void CopyTo(IScriptMember[] array, int arrayIndex) {
            members.CopyTo(array, arrayIndex);
        }

        public bool Remove(IScriptMember item) {
            return members.Remove(item);
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

        public object Clone() {
            return new ScriptObject(this);
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
    }

    public interface IScriptObjectManager : IDictionary<string, ScriptObject>, IEnumerable<ScriptObject>, IEnumerable {
        IEnumerable<ScriptObject> ScriptObjects { get; }
        ScriptObject GetScriptObject(string scriptName, bool searchSourceFolders);
    }

    public sealed partial class ScriptObject {
        private static ScriptObject TryParse(string scriptName, TextReader stream) {
            TokenScanner scanner = TokenScanner.IncludeAllParsers();

            ScriptObject scriptObject = new ScriptObject(scriptName);
            Manager.Add(scriptName, scriptObject);

            string textLine;
            var parsedLineQueue = new List<Token>();
            while ((textLine = stream.ReadLine()) != null) {
                var parsedLine = new List<Token>();
                scanner.ScanLine(textLine, parsedLineQueue);
                parsedLineQueue.AddRange(parsedLine);

                if (parsedLine.Count == 0 || parsedLine.Any(t => t.ExtendsLine == true)) {
                    continue;
                }

                if (!scriptObject.Info.IsValid) {
                    scriptObject.members.Clear();
                    if (!scriptObject.Info.TryParse(parsedLineQueue, 0)) {
                        return null;
                    }
                }
                else {
                    IScriptMember member;
                    if (PropertyMember.TryParse(parsedLineQueue, 0, out member) ||
                        FunctionMember.TryParse(parsedLineQueue, 0, out member) ||
                        EventMember.TryParse(parsedLineQueue, 0, out member)) {
                        scriptObject.members.Add(member);
                    }
                }

                parsedLineQueue.Clear();
            }

            return scriptObject;
        }
        private static ScriptObject TryParse(string filePath) {
            if (String.IsNullOrEmpty(filePath)) {
                return null;
            }
            return TryParse(Path.GetFileNameWithoutExtension(filePath), new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)));
        }

        public static IScriptObjectManager Manager {
            get { return ScriptObjectManager.Instance; }
        }
        private class ScriptObjectManager : IScriptObjectManager, IDictionary<string, ScriptObject>, IReadOnlyDictionary<string, ScriptObject>, IEnumerable<ScriptObject>, IEnumerable {
            private static ScriptObjectManager instance = null;
            private static object syncRoot = new object();

            public static ScriptObjectManager Instance {
                get {
                    if (instance == null) {
                        lock (syncRoot) {
                            if (instance == null) {
                                instance = new ScriptObjectManager();
                            }
                        }
                    }
                    return instance;
                }
            }

            private SortedList<string, ScriptObject> scriptObjectList = new SortedList<string, ScriptObject>(StringComparer.OrdinalIgnoreCase);

            public IEnumerable<ScriptObject> ScriptObjects {
                get { return scriptObjectList.Values; }
            }

            public ScriptObject this[string key] {
                get { return scriptObjectList[key]; }
            }
            ScriptObject IDictionary<string, ScriptObject>.this[string key] {
                get { return scriptObjectList[key]; }
                set { scriptObjectList[key] = value; }
            }

            ICollection<string> IDictionary<string, ScriptObject>.Keys {
                get { return scriptObjectList.Keys; }
            }
            ICollection<ScriptObject> IDictionary<string, ScriptObject>.Values {
                get { return this.scriptObjectList.Values; }
            }
            public IEnumerable<string> Keys {
                get { return scriptObjectList.Keys; }
            }
            public IEnumerable<ScriptObject> Values {
                get { return this.scriptObjectList.Values; }
            }

            public int Count {
                get { return scriptObjectList.Count; }
            }
            public bool IsReadOnly {
                get { return ((IDictionary<string, ScriptObject>)scriptObjectList).IsReadOnly; }
            }

            public ScriptObject GetScriptObject(string scriptName, bool searchSourceFolders) {
                ScriptObject scriptObject;
                if (searchSourceFolders ? this.TryGetValue(scriptName, out scriptObject) : scriptObjectList.TryGetValue(scriptName, out scriptObject)) {
                    return scriptObject;
                }
                return null;
            }

            public void Add(string key, ScriptObject value) {
                scriptObjectList.Add(key, value);
            }
            void ICollection<KeyValuePair<string, ScriptObject>>.Add(KeyValuePair<string, ScriptObject> item) {
                ((IDictionary<string, ScriptObject>)scriptObjectList).Add(item);
            }

            public bool ContainsKey(string key) {
                return scriptObjectList.ContainsKey(key);
            }
            bool ICollection<KeyValuePair<string, ScriptObject>>.Contains(KeyValuePair<string, ScriptObject> item) {
                return scriptObjectList.Contains(item);
            }
            public bool TryGetValue(string key, out ScriptObject value) {
                value = null;
                if (scriptObjectList.TryGetValue(key, out value)) {
                    return true;
                }

                string filePath = PapyrusEditor.Instance.CurrentGame.FindFileInSourceFolders(key);
                value = TryParse(filePath);

                return value != null;
            }

            void ICollection<KeyValuePair<string, ScriptObject>>.CopyTo(KeyValuePair<string, ScriptObject>[] array, int arrayIndex) {
                ((IDictionary<string, ScriptObject>)scriptObjectList).CopyTo(array, arrayIndex);
            }

            public bool Remove(string key) {
                return scriptObjectList.Remove(key);
            }
            bool ICollection<KeyValuePair<string, ScriptObject>>.Remove(KeyValuePair<string, ScriptObject> item) {
                return ((IDictionary<string, ScriptObject>)scriptObjectList).Remove(item);
            }
            public void Clear() {
                scriptObjectList.Clear();
            }

            IEnumerator<KeyValuePair<string, ScriptObject>> IEnumerable<KeyValuePair<string, ScriptObject>>.GetEnumerator() {
                return scriptObjectList.GetEnumerator();
            }
            IEnumerator<ScriptObject> IEnumerable<ScriptObject>.GetEnumerator() {
                return scriptObjectList.Values.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator() {
                return scriptObjectList.Values.GetEnumerator();
            }
        }
    }

    internal sealed class ScriptObjectParser : TokenParser {
        /*
        public bool TryParse(SnapshotSpan sourceSnapshotSpan, ref TokenScannerState state, TokenInfo tokenInfo) {
            if (state == TokenScannerState.Text) {
                string text = sourceSnapshotSpan.GetText();
                int nextDelimiter = Delimiter.FindNext(text, 0);
                if (nextDelimiter > 0) {
                    ScriptObject scriptObject;
                    if (ScriptObject.TryParse(text.Substring(0, nextDelimiter), out scriptObject)) {
                        token.Type = scriptObject;
                        token.Span = new SnapshotSpan(sourceSnapshotSpan.Snapshot, sourceSnapshotSpan.Subspan(0, scriptObject.Text.Length));
                        return true;
                    }
                }
            }

            return false;
        }
        */
        public override bool TryParse(string sourceTextSpan, ref TokenScannerState state, IEnumerable<Token> previousTokens, out Token token) {
            if (state == TokenScannerState.Text) {
                int nextDelimiter = Delimiter.FindNext(sourceTextSpan, 0);
                if (nextDelimiter > 0) {
                    token = ScriptObject.Manager.GetScriptObject(sourceTextSpan.Substring(0, nextDelimiter), true);
                    return token != null;
                }
            }
            token = null;
            return false;
        }
    }
}
