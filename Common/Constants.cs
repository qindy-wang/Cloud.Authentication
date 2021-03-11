using System;
using System.Collections.Generic;
using System.Text;

namespace Cloud.Authentication.Common
{
    public class ExceptionConstants
    {
        public static string NullOrEmpty = "The arguments is null or empty.";
        public static string InvalidParameter = "Invalid expirationSettings: {0} to set token cache.";
        public static string InvalidToken = "The access token is invalid.";
    }

    public enum ErrorCode
    { 
       None = 0,
       InvalidToken = 101,
       InvalidArguments = 102,
       BadRequest = 500,
       Unknown = 111
    }
}
