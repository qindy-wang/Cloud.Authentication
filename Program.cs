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
        private static string _username = "";
        private static string _password = "";
        private static string _clientId = "";
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
