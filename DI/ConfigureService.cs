using Cloud.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

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
