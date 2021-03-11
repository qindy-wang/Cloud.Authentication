using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public interface ITokenCacheService
    {
        Task<string> TryGetValue(string username, string password, TokenType tokenType, string clientId = "");

        Task<string> TryGetValue(string clientId, string clientSecret);

        void TrySetValue<T>(string key, T obj, object expirationSettings);

        void TryRemoveValue(string key);
    }
}
