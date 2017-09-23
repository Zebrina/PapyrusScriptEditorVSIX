using Microsoft.VisualStudio.Text;
using Papyrus.Language;
using Papyrus.Language.Parsing;
using Papyrus.Language.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Papyrus.Features {
    internal class BackgroundParser {
        private static BackgroundParser instance;
        private static object syncRoot = new object();
        public static BackgroundParser Singleton {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new BackgroundParser();
                        }
                    }
                }
                return instance;
            }
        }

        private ITextSnapshot lastParsedSnapshot = null;
        private TokenSnapshot resultTokenSnapshot = null;
        private object parseLock = new object();

        public IReadOnlyTokenSnapshot TokenSnapshot {
            get { return resultTokenSnapshot; }
        }

        public void ForceReParse(ITextSnapshot snapshot) {
            lock (parseLock) {
                TokenScanner scanner = new TokenScanner();
                scanner.AddParser(new PapyrusBlockCommentParser());
                scanner.AddParser(new PapyrusLineCommentParser());
                scanner.AddParser(new PapyrusDocumentationParser());
                scanner.AddParser(new PapyrusStringLiteralParser());
                scanner.AddParser(new PapyrusNumericLiteralParser());
                scanner.AddParser(new PapyrusOperatorParser());
                scanner.AddParser(new PapyrusDelimiterParser());
                scanner.AddParser(new PapyrusKeywordParser());
                scanner.AddParser(new PapyrusScriptObjectParser());
                scanner.AddParser(new PapyrusIdentifierParser());

                resultTokenSnapshot = new TokenSnapshot(snapshot);
                var parsedLineQueue = new List<PapyrusTokenInfo>();
                foreach (var line in snapshot.Lines) {
                    var parsedLine = new List<PapyrusTokenInfo>();
                    scanner.ScanLine(line, parsedLine);
                    parsedLineQueue.AddRange(parsedLine);

                    if (parsedLine.Count == 0 || parsedLine.Any(t => t.Type.IsLineExtension)) {
                        continue;
                    }
                    
                    resultTokenSnapshot.Add(new TokenSnapshotLine(parsedLineQueue));
                    parsedLineQueue.Clear();
                }
                lastParsedSnapshot = snapshot;
            }
        }
        public void RequestReParse(ITextSnapshot snapshot) {
            if (snapshot != lastParsedSnapshot) {
                ForceReParse(snapshot);
            }
        }
    }
}
