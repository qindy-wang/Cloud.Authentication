using Cloud.Authentication.DI;
using Cloud.Authentication.Services;
using Cloud.Authentication.Services.Cache;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloud.Authentication
{
    class Program
    {
        private static string _username = "admin@M365x677264.onmicrosoft.com";
        private static string _password = "5kep7353bC";
        private static string _clientId = "d6e01331-be4e-4114-86f1-09f2a9252679";
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            var provider = services.ConfigureServices().BuildServiceProvider();
            //var httpClientService = provider.GetService<IHttpClientService>();
            //var httpClient = httpClientService?.GetHttpClient(_username, _password, TokenType.SharePoint, _clientId).GetAwaiter().GetResult();

            var tokenCacheService = provider.GetService<TokenCacheService>();           
            var token = tokenCacheService?.TryGetValue(_username, _password, TokenType.AzureAD).GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}
