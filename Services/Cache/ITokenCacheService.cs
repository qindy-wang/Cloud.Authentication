using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public interface ITokenCacheService
    {
        /// <summary>
        /// ClientId & ResourceUrl only for SharePoint token type, others can ignore or input empty
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="tokenType"></param>
        /// <param name="clientId"></param>
        /// <param name="resourceUrl"></param>
        /// <returns></returns>
        Task<string> TryGetValue(string username, string password, TokenType tokenType, string clientId = "", string resourceUrl = "");

        Task<string> TryGetValue(string clientId, string clientSecret);

        void TrySetValue<T>(string key, T obj, object expirationSettings);

        void TryRemoveValue(string key);
    }
}
