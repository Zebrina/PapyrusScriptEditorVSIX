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

        private static TokenScanner scanner = TokenScanner.IncludeAllParsers();

        private ITextSnapshot lastParsedSnapshot = null;
        private TokenSnapshot resultTokenSnapshot = null;

        public IReadOnlyTokenSnapshot TokenSnapshot {
            get { return resultTokenSnapshot; }
        }

        public void RequestParse(ITextSnapshot snapshot) {
            lock (scanner) {
                if (snapshot != lastParsedSnapshot) {
                    resultTokenSnapshot = new TokenSnapshot();
                    List<TokenInfo> tokens = new List<TokenInfo>();
                    foreach (var line in snapshot.Lines) {
                        scanner.ScanLine(line, tokens);
                        if (tokens.Count > 0 && tokens.Last().Type.ExtendsLine == false) {
                            resultTokenSnapshot.Add(new TokenSnapshotLine(line, tokens));
                            tokens.Clear();
                        }
                    }
                    lastParsedSnapshot = snapshot;
                }
            }
        }
    }
}
