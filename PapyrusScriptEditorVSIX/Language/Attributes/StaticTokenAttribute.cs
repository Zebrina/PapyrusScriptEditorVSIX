using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Papyrus.Language {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StaticTokenAttribute : Attribute {
        private Type[] allowedGameTypes;

        public StaticTokenAttribute(params Type[] allowedGameTypes) {
            this.allowedGameTypes = allowedGameTypes;
        }

        public bool AllowedInGame(IGameInfo gameInfo) {
            return allowedGameTypes.Length == 0 || allowedGameTypes.Contains(gameInfo.GetType());
        }
    }
}
