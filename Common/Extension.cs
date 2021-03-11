using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Cloud.Authentication.Common
{
    public static class Extenstion
    {
        public static SecureString ToSecureString(this string value)
        {
            var secureValue = new SecureString();
            Array.ForEach(value.ToCharArray(), secureValue.AppendChar);
            return secureValue;
        }
    }
}
