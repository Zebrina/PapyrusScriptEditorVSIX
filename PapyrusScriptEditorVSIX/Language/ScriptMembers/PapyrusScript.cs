using Papyrus.Common;
using Papyrus.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.ScriptMembers {
    /// <summary>
    /// Top-level 'script member'. Basically the script with all it's properties, functions, events, etc.
    /// All ScriptObject tokens should link to a papyrus script.
    /// </summary>
    public sealed class PapyrusScript : IPapyrusScriptMember {
        private PapyrusScript parentScript;
        private HashSet<string> memberKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<PapyrusScriptMemberType, ICollection<IPapyrusScriptMember>> memberValues;

        public PapyrusScript() {
            this.memberValues = new Dictionary<PapyrusScriptMemberType, ICollection<IPapyrusScriptMember>>();
            foreach (PapyrusScriptMemberType type in Enum.GetValues(typeof(PapyrusScriptMemberType))) {
                memberValues.Add(type, new List<IPapyrusScriptMember>());
            }
        }

        PapyrusScriptMemberType IPapyrusScriptMember.Type {
            get { return PapyrusScriptMemberType.PapyrusScript; }
        }
        public string ScriptName { get; private set; }
        public IEnumerable<IPapyrusScriptMember> GetMembers(string filter) {
            foreach (var memberTypeList in memberValues) {
                foreach (var member in memberTypeList.Value.Where(m => String.IsNullOrWhiteSpace(filter) || m.ScriptName.ContainsIgnoreCase(filter))) {
                    yield return member;
                }
            }
        }
        public bool AddMember(IPapyrusScriptMember member) {
            if (memberKeys.Contains(member.ScriptName)) {
                return false;
            }

            ICollection<IPapyrusScriptMember> members;
            if (!memberValues.TryGetValue(PapyrusScriptMemberType.PapyrusImport, out members)) {
                members = new List<IPapyrusScriptMember>();
                memberValues.Add(PapyrusScriptMemberType.PapyrusImport, members);
            }

            members.Add(member);
            memberKeys.Add(member.ScriptName);

            return true;
        }
        public bool RemoveMember(string memberName) {
            if (memberKeys.Contains(memberName)) {
                IPapyrusScriptMember element = null;
                var memberTypeList = memberValues.First(mList => mList.Value.Any(m => {
                    if (m.ScriptName.Equals(memberName, StringComparison.OrdinalIgnoreCase)) {
                        element = m;
                        return true;
                    }
                    return false;
                }));

                memberTypeList.Value.Remove(element);
                if (memberTypeList.Value.Count == 0) {
                    memberValues.Remove(memberTypeList.Key);
                }

                return true;
            }

            return false;
        }

        public bool ConvertToText(StringBuilder stringBuilder, TextFormatInfo formatInfo) {
            return false;
        }
    }

    public sealed class PapyrusScriptManager {
        private static PapyrusScriptManager instance = null;
        private static object syncObj = new object();
        public static PapyrusScriptManager Instance {
            get {
                if (instance == null) {
                    lock (syncObj) {
                        if (instance == null) {
                            instance = new PapyrusScriptManager();
                        }
                    }
                }
                return instance;
            }
        }

        private Dictionary<string, PapyrusScript> loadedScripts = new Dictionary<string, PapyrusScript>(StringComparer.OrdinalIgnoreCase);

        public PapyrusScript GetScript(string scriptName) {
            PapyrusScript script;
            if (loadedScripts.TryGetValue(scriptName, out script)) {
                return script;
            }
            return null;
        }
        public void AddScript(PapyrusScript script) {
            if (!loadedScripts.ContainsKey(script.ScriptName)) {
                loadedScripts.Add(script.ScriptName, script);
            }
        }
        public bool RemoveScript(PapyrusScript script) {
            return loadedScripts.Remove(script.ScriptName);
        }
        public bool RemoveScript(string scriptName) {
            return loadedScripts.Remove(scriptName);
        }

        public bool LoadScript(string fileName) {
            return false;
        }
    }
}
