using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    public enum ScannerStates {
        GlobalScope,
        BlockComment,
        Documentation,
        ParameterList,
        PropertyGetScope,
        PropertySetScope,
        FunctionScope,
        EventScope,
        GroupScope,
        StructScope,
    }

    public abstract class Scanner<T> {
        private ICollection<T> handlers = new List<T>();
        private Stack<ScannerStates> state = new Stack<ScannerStates>();

        public ScannerStates TopLevelState {
            get { return state.Count == 0 ? ScannerStates.GlobalScope : state.Peek(); }
        }

        public bool SetState(ScannerStates newState) {
            if (TopLevelState == newState) {
                return false;
            }
            state.Push(newState);
            return true;
        }
        public bool ClearState(ScannerStates oldState) {
            if (TopLevelState != oldState) {
                return false;
            }
            state.Pop();
            return true;
        }

        public static Scanner<T> operator +(Scanner<T> scanner, T matcher) {
            if (scanner != null) {
                scanner.handlers.Add(matcher);
            }
            return scanner;
        }
    }
}
