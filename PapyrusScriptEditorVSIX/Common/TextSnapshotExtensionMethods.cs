using Microsoft.VisualStudio.Text;
using System;
using System.Diagnostics;

namespace Papyrus.Common {
    /*
    [DebuggerStepThrough]
    internal static class TextSnapshotLineExtensionMethods {
        public static SnapshotSpan ToSpan(this ITextSnapshotLine line) {
            return new SnapshotSpan(line.Start, line.End);
        }
    }
    */

    [DebuggerStepThrough]
    internal static class SnapshotSpanExtensionMethods {
        public static SnapshotSpan Subspan(this SnapshotSpan span, int offset, int length) {
            return new SnapshotSpan(span.Snapshot, new Span(span.Start.Position + offset, length));
        }
        public static SnapshotSpan Subspan(this SnapshotSpan span, int offset) {
            return span.Subspan(offset, span.Length - offset);
        }
        /*
        public static SnapshotSpan Subspan(this SnapshotSpan span, SnapshotPoint offset, int length) {
            return new SnapshotSpan(offset, length);
        }
        */
        public static SnapshotSpan Subspan(this SnapshotSpan span, SnapshotPoint offset) {
            if (offset > span.End) {
                return new SnapshotSpan(span.End, 0);
            }
            return new SnapshotSpan(offset, span.End);
        }

        public static SnapshotSpan Ignore(this SnapshotSpan span, Predicate<char> pred) {
            int offset = span.Start.Position;
            for (int i = 0; i < span.Length; ++i) {
                if (!pred.Invoke(span.Snapshot[i + offset])) {
                    return new SnapshotSpan(span.Start.Add(i), span.End);
                }
            }
            return new SnapshotSpan(span.End, 0);
        }
        public static SnapshotSpan Ignore(this SnapshotSpan span) {
            return span.Ignore(c => Char.IsWhiteSpace(c));
        }
    }
}
