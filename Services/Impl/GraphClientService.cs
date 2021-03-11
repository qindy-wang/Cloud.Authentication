using Cloud.Authentication.Services.Cache;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services
{
    public class GraphClientService : IGraphClientService
    {
        private readonly ITokenCacheService _tokenCacheService;
        public GraphClientService(ITokenCacheService tokenCacheService)
        {
            this._tokenCacheService = tokenCacheService;
        }

        public GraphServiceClient GetGraphServiceClientByApp(string tenantId, string clientId, string clientSecret)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();
            var credentialProvider = new ClientCredentialProvider(confidentialClientApplication);
            var graphClient = new GraphServiceClient(credentialProvider);
            return graphClient;
        }

        public async Task<GraphServiceClient> GetGraphServiceClientByIdentity(string username, string password)
        {
            var accessToken = await _tokenCacheService.TryGetValue(username,password,TokenType.AzureAD);

            var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async (request) => {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                await Task.FromResult(true);
            }));
            return graphClient;
        }
    }   
}
