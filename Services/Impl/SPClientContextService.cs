using Cloud.Authentication.Services.Cache;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task<ClientContext> GetSPClientContextInstance(string username, string password, string clientId)
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
    }
}
