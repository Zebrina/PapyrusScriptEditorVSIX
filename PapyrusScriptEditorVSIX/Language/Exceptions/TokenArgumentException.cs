using System;

namespace Papyrus.Language.Exceptions {
    public enum TokenArgumentExceptionType {
        NotCompileTimeConstant,
        NotVariableType,
    }

    [Serializable]
    public class TokenArgumentException : ArgumentException {
        public TokenArgumentException(TokenArgumentExceptionType exceptionType, Token value, string paramName) :
            base(TranslateMessage(exceptionType, value), paramName) {
        }

        private static string TranslateMessage(TokenArgumentExceptionType exceptionType, Token value) {
            switch (exceptionType) {
                case TokenArgumentExceptionType.NotCompileTimeConstant:
                    return String.Format("{0} is not a compile time constant token.", value);
                case TokenArgumentExceptionType.NotVariableType:
                    return String.Format("{0} is not a variable type token", value);
                default:
                    return "Undefined exception.";
            }
        }
    }
}
