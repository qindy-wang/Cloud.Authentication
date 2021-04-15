using Cloud.Authentication.Common;
using Cloud.Authentication.Exeption;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Cloud.Authentication.Services
{
    public class TokenService : ITokenService
    {
        private string _tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/token";
        public async Task<string> GetAccessToken(string username, string password, TokenType tokenType, string clientId = "", string resourceUrl= "")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new TokenException(ErrorCode.InvalidArguments, ExceptionConstants.NullOrEmpty);
            }
            switch (tokenType)
            {
                case TokenType.AzureAD:
                case TokenType.EndpointManager:
                    return await GetAADAccessToken(username, password);
                case TokenType.SharePoint:
                    return await GetSPAccessToken(username, password, clientId, resourceUrl);
                default:
                    return string.Empty;
            }
        }

        public async Task<string> GetAppAccessToken(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new TokenException(ErrorCode.InvalidArguments, ExceptionConstants.NullOrEmpty);
            }
            var resource = $"scope=https://graph.microsoft.com/.default&grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";
            return await GetAccessToken(resource);
        }

        private async Task<string> GetSPAccessToken(string username, string password, string clientId, string resourceUrl)
        {
            if (string.IsNullOrEmpty(resourceUrl))
            {
                resourceUrl = $"https://{username.Substring(username.IndexOf("@") + 1, username.IndexOf(".") - username.IndexOf("@") - 1)}-admin.sharepoint.com/";
            }
            try
            {
                var uri = new Uri(resourceUrl);
                string _resource = $"{uri.Scheme}://{uri.DnsSafeHost}";
                var resource = $"resource={_resource}&client_id={clientId}&grant_type=password&username={HttpUtility.UrlEncode(username)}&password={HttpUtility.UrlEncode(password)}";
                return await GetAccessToken(resource);
            }
            catch (System.UriFormatException e)
            {
                throw new UriFormatException(e.Message, e);
            }
        }

        private async Task<string> GetAADAccessToken(string username, string password)
        {
            var resource = $"resource=74658136-14ec-4630-ad9b-26e160ff0fc6&client_id=1950a258-227b-4e31-a9cf-717495945fc2&grant_type=password&username={username}&password={password}";
            return await GetAccessToken(resource);
        }

        private async Task<string> GetAccessToken(string resource)
        {
            var httpClient = new HttpClient();
            using (var stringContent = new StringContent(resource, Encoding.UTF8, "application/x-www-form-urlencoded"))
            {
                try
                {
                    var result = await httpClient.PostAsync(_tokenEndpoint, stringContent).ContinueWith((response) =>
                    {
                        return response.Result.Content.ReadAsStringAsync().Result;
                    }).ConfigureAwait(false);

                    var tokenResult = JsonSerializer.Deserialize<JsonElement>(result);
                    var token = tokenResult.GetProperty("access_token").GetString();
                    return token;
                }
                catch (Exception ex)
                {
                    throw new TokenException(ErrorCode.BadRequest, ex.Message);
                }
            }
        }
    }
}
