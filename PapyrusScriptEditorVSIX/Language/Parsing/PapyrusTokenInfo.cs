using Microsoft.VisualStudio.Text;
using Papyrus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language.Parsing {
    public sealed class PapyrusTokenInfo : IEquatable<PapyrusTokenInfo> {
        public IPapyrusToken Type { get; set; }
        public SnapshotSpan Span { get; set; }

        public PapyrusTokenInfo(IPapyrusToken type, SnapshotSpan span) {
            this.Type = type;
            this.Span = span;
        }
        public PapyrusTokenInfo() :
            this(null, default(SnapshotSpan)) {
        }

        public override int GetHashCode() {
            return Hash.GetMemberwiseHashCode(Type, Span);
        }
        public override bool Equals(object obj) {
            return obj is PapyrusTokenInfo && this.Equals((PapyrusTokenInfo)obj);
        }
        public bool Equals(PapyrusTokenInfo other) {
            return this.Type.Equals(other.Type) && this.Span.Equals(other.Span);
        }
        public bool TypeEquals(PapyrusTokenInfo other) {
            return Type.Equals(other.Type);
        }
    }
}
