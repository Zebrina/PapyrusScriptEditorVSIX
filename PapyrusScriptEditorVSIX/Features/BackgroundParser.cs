using Microsoft.VisualStudio.Text;
using Papyrus.Language;
using Papyrus.Language.Components;
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

        public void RequestParse(ITextSnapshot snapshot) {
            if (snapshot != lastParsedSnapshot) {
                lock (parseLock) {
                    if (snapshot != lastParsedSnapshot) {
                        TokenScanner scanner = TokenScanner.IncludeAllParsers();
                        resultTokenSnapshot = new TokenSnapshot(snapshot);
                        List<PapyrusTokenInfo> parsedLineQueue = new List<PapyrusTokenInfo>();
                        foreach (var line in snapshot.Lines) {
                            var parsedLine = new List<PapyrusTokenInfo>();
                            scanner.ScanLine(line, parsedLine);
                            parsedLineQueue.AddRange(parsedLine);

                            if (parsedLine.Count > 0 && parsedLine.Any(t => t.Type.ExtendsLine)) {
                                continue;
                            }

                            if (parsedLineQueue.Count > 0) {
                                resultTokenSnapshot.Add(new TokenSnapshotLine(parsedLineQueue));
                                parsedLineQueue.Clear();
                            }
                        }
                        lastParsedSnapshot = snapshot;
                    }
                }
            }
        }
    }
}
