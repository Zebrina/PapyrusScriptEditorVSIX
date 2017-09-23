﻿using Papyrus.Common;
using Papyrus.Language_NEW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language_NEW.ScriptMembers {
    /// <summary>
    /// Top-level 'script member'. Basically the script with all it's properties, functions, events, etc.
    /// All ScriptObject tokens should link to a papyrus script.
    /// </summary>
    public class PapyrusScript : IPapyrusScriptMember {
        private string scriptName;
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
        public string Name {
            get { return scriptName; }
        }
        public IEnumerable<IPapyrusScriptMember> GetMembers(string filter) {
            foreach (var memberTypeList in memberValues) {
                foreach (var member in memberTypeList.Value.Where(m => String.IsNullOrWhiteSpace(filter) || m.Name.ContainsIgnoreCase(filter))) {
                    yield return member;
                }
            }
        }
        public bool AddMember(IPapyrusScriptMember member) {
            if (memberKeys.Contains(member.Name)) {
                return false;
            }

            ICollection<IPapyrusScriptMember> members;
            if (!memberValues.TryGetValue(PapyrusScriptMemberType.PapyrusImport, out members)) {
                members = new List<IPapyrusScriptMember>();
                memberValues.Add(PapyrusScriptMemberType.PapyrusImport, members);
            }

            members.Add(member);
            memberKeys.Add(member.Name);

            return true;
        }
        public bool RemoveMember(string memberName) {
            if (memberKeys.Contains(memberName)) {
                IPapyrusScriptMember element = null;
                var memberTypeList = memberValues.First(mList => mList.Value.Any(m => {
                    if (m.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase)) {
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
}
