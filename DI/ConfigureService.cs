using Cloud.Authentication.Services;
using Cloud.Authentication.Services.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloud.Authentication.DI
{
    public static class ConfigureService
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IHttpClientService, HttpClientService>();
            services.AddMemoryCache();
            services.AddTransient<CloudMemoryCache>();
            services.AddTransient<ITokenCacheService, TokenCacheService>();
            return services;
        }
    }
}
