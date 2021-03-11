using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public interface IHttpClientService
    {
        /// <summary>
        /// Get Azure AD backend access token impersonate user login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="tokenType"></param>
        /// <param name="clientId">ClientId is required when tokenType = SharePoint</param>
        /// <returns></returns>
        Task<HttpClient> GetHttpClient(string username, string password, TokenType tokenType, string clientId = "");

        Task<HttpClient> GetGraphHttpClient(string clientId, string clientSecret);
    }
}
