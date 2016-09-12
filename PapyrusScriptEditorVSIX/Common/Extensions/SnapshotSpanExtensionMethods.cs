using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Common.Extensions {
    internal static class SnapshotSpanExtended {
        public static SnapshotSpan Subspan(this SnapshotSpan value, int offset, int length) {
            return new SnapshotSpan(value.Snapshot, new Span(value.Start.Position + offset, length));
        }
        public static SnapshotSpan Subspan(this SnapshotSpan value, int offset) {
            return value.Subspan(offset, value.Length - offset);
        }

        public static SnapshotSpan Trim(this SnapshotSpan value) {
            string text = value.GetText();

            int offset;
            for (offset = 0; offset < text.Length; ++offset) {
                if (!Char.IsWhiteSpace(text[offset])) {
                    break;
                }
            }

            int length = text.Length - offset;
            for (; length >= 0; --length) {
                if (!Char.IsWhiteSpace(text[offset + (length - 1)])) {
                    break;
                }
            }

            return new SnapshotSpan(value.Start + offset, length);
        }
    }
}
