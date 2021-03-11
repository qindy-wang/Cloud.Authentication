using Cloud.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloud.Authentication.Exeption
{
    public class TokenException: Exception
    {
        protected ErrorCode errorCode;

        public TokenException() : base()
        {
            errorCode = ErrorCode.InvalidToken;
        }

        public TokenException(string message) 
            : base(message)
        {
            errorCode = ErrorCode.InvalidToken;
        }

        public TokenException(string message, ErrorCode errorCode, Exception innerException) 
            : base(message, innerException)
        {
            this.errorCode = errorCode;
        }

        public TokenException(ErrorCode errorCode, string message)
            : base(message)
        {
            this.errorCode = errorCode;
        }
    }
}
