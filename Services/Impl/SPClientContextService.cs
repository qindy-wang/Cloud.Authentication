using Microsoft.SharePoint.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services.Impl
{
    public class SPClientContextService
    {
        private readonly ITokenCacheService _tokenCacheService;

        public SPClientContextService(ITokenCacheService tokenCacheService)
        {
            this._tokenCacheService = tokenCacheService;
        }

        public async Task<ClientContext> GetSPClientContext(string username, string password, string clientId)
        {
            var accessToken = await _tokenCacheService.TryGetValue(username, password, TokenType.SharePoint, clientId);
            var spAdminUrl = $"https://{username.Substring(username.IndexOf("@") + 1, username.IndexOf(".") - username.IndexOf("@") - 1)}-admin.sharepoint.com/";
            var context = new ClientContext(new Uri(spAdminUrl));
            context.ExecutingWebRequest += (sender, e) =>
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
            };
            return context;
        }

        public async Task<ClientContext> GetSPClientContext(string username, string password, string clientId, string siteUrl)
        {
            var accessToken = await _tokenCacheService.TryGetValue(username, password, TokenType.SharePoint, clientId, siteUrl);
            var context = new ClientContext(new Uri(siteUrl));
            context.ExecutingWebRequest += (sender, e) =>
            {
                e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + accessToken;
            };
            return context;
        }
    }
}
