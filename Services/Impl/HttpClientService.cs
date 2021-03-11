using Cloud.Authentication.Services.Cache;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly ITokenCacheService _tokenCacheService;
        public HttpClientService(ITokenCacheService tokenCacheService)
        {
            this._tokenCacheService = tokenCacheService;
        }

        public async Task<HttpClient> GetHttpClient(string username, string password, TokenType tokenType, string clientId = "")
        {
            var accessToken = await _tokenCacheService.TryGetValue(username, password, tokenType, clientId);
            return BuildHttpClient(accessToken);
        }

        public async Task<HttpClient> GetGraphHttpClient(string clientId, string clientSecret)
        {
            var accessToken = await _tokenCacheService.TryGetValue(clientId, clientSecret);
            return BuildHttpClient(accessToken);
        }

        private HttpClient BuildHttpClient(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            httpClient.DefaultRequestHeaders.Add("x-ms-client-request-id", $"{new Guid()}");
            httpClient.DefaultRequestHeaders.Add("x-ms-correlation-id", $"{new Guid()}");
            return httpClient;
        }
    }
}
