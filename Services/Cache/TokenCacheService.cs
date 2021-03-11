using Cloud.Authentication.Common;
using Cloud.Authentication.Exeption;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Authentication.Services.Cache
{
    public class TokenCacheService: ITokenCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;
        private string _confidentialTokenKey {get;set;} = string.Empty;
        private string _appTokenKey { get; set; } = string.Empty;

        public TokenCacheService(IMemoryCache memoryCache, ITokenService tokenService)
        {
            this._cache = memoryCache;
            this._tokenService = tokenService;
        }

        public async Task<string> TryGetValue(string username, string password, TokenType tokenType, string clientId = "")
        {
            var token = string.Empty;
            _confidentialTokenKey = BuildCacheKey(username, password, tokenType, clientId);
            if (!_cache.TryGetValue(_confidentialTokenKey, out token))
            {
                token = await _tokenService.GetAccessToken(username, password, tokenType, clientId);
                this.SetCache(_confidentialTokenKey, token);
            }
            return token;
        }

        public void TrySetValue<T>(string key, T obj, object expirationSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(_cache.Get<string>(key)))
                {
                    if (expirationSettings == null)
                    {
                        _cache.Set(_confidentialTokenKey, obj);
                    }
                    if (expirationSettings is TimeSpan)
                    {
                        _cache.Set(_confidentialTokenKey, obj, (TimeSpan)expirationSettings);
                    }
                    else if (expirationSettings is DateTimeOffset)
                    {
                        _cache.Set(key, obj, (DateTimeOffset)expirationSettings);
                    }

                    else if (_cache is MemoryCacheEntryOptions)
                    {
                        _cache.Set(key, obj, (MemoryCacheEntryOptions)_cache);
                    }
                    else if (_cache.GetType().GetInterface(typeof(IChangeToken).Name) != null)
                    {
                        _cache.Set(key, obj, (IChangeToken)_cache);
                    }
                    else
                    {
                        throw new TokenException(ErrorCode.InvalidArguments, string.Format(ExceptionConstants.InvalidParameter, $"{expirationSettings}"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TokenException(ex.Message, ErrorCode.Unknown, ex);
            }
        }

        public async Task<string> TryGetValue(string clientId, string clientSecret)
        {
            var token = string.Empty;
            _appTokenKey = BuildCacheKey(clientId, clientSecret);
            return await _cache.GetOrCreateAsync(_appTokenKey, async entry =>
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    if (jwtToken.ValidTo.Ticks > DateTime.UtcNow.Ticks)
                    {
                        var expiration = new TimeSpan(jwtToken.ValidTo.Ticks - DateTime.UtcNow.Ticks);
                        if (expiration.TotalMinutes > 10)
                        {
                            entry.SlidingExpiration = expiration.Subtract(new TimeSpan(0, 5, 0));
                        }
                        else if (expiration.TotalMinutes > 2)
                        {
                            entry.SlidingExpiration = expiration.Subtract(new TimeSpan(0, 1, 0));
                        }
                        else
                        {
                            entry.SlidingExpiration = expiration;
                        }
                    }
                }
                token = await _tokenService.GetAppAccessToken(clientId, clientSecret);
                return token;
            });
        }

        public void TryRemoveValue(string key)
        {
            _cache.Remove(key);
        }

        public void CreateDependencyEntries(TimeSpan timeSpan)
        {
            var cts = new CancellationTokenSource(timeSpan);

            using (var entry = _cache.CreateEntry($"{_confidentialTokenKey}_parent"))
            {
                entry.Value = DateTime.Now;
                entry.RegisterPostEvictionCallback(EvictionCallback, this);
                //auto expiration
                var memoryOptions = new MemoryCacheEntryOptions()
                    .AddExpirationToken(new CancellationChangeToken(cts.Token))
                    .SetSize(1);
                //remove cache
                //new CancellationChangeToken(cts.Token)
                //_cache.Set($"{_confidentialTokenKey}_cts", cts);
                _cache.Set($"{_confidentialTokenKey}_child", DateTime.Now, memoryOptions);
            }
        }

        public void RemoveDependencyEntry()
        {
            _cache.Get<CancellationTokenSource>($"{_confidentialTokenKey}_cts").Cancel();
        }

        public void GetParentEntry()
        {
            var childCache = _cache.Get($"{_confidentialTokenKey}_child");
            var parentCache = _cache.Get($"{_confidentialTokenKey}_parent");
        }

        //Cache expiration callback method
        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            Console.WriteLine($"key: {key}, reason: {reason}");
        }

        private string BuildCacheKey(params object[] parameters)
        {
           return string.Join("-", parameters);
        }

        private void SetCache(string key, string token)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                if (jwtToken.ValidTo.Ticks > DateTime.UtcNow.Ticks)
                {
                    var expiration = new TimeSpan(jwtToken.ValidTo.Ticks - DateTime.UtcNow.Ticks);
                    if (expiration.TotalMinutes > 10)
                    {
                        cacheEntryOptions.SetSlidingExpiration(expiration.Subtract(new TimeSpan(0, 5, 0)));
                    }
                    else if (expiration.TotalMinutes > 2)
                    {
                        cacheEntryOptions.SetSlidingExpiration(expiration.Subtract(new TimeSpan(0, 1, 0)));
                    }
                    else 
                    {
                        cacheEntryOptions.SetSlidingExpiration(expiration);
                    }
                }
                cacheEntryOptions
                    .SetPriority(CacheItemPriority.High)
                    .RegisterPostEvictionCallback(callback: EvictionCallback, state: this)
                    .SetSize(1);
                _cache.Set(_confidentialTokenKey, token, cacheEntryOptions);
            }
            else 
            {
                throw new TokenException(ErrorCode.InvalidToken, ExceptionConstants.InvalidToken);
            }
        }
    }

    public class CloudMemoryCache
    {
        public MemoryCache Cache { get; set; }
        public CloudMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 10,
                CompactionPercentage = 0.33,
                ExpirationScanFrequency = TimeSpan.FromMinutes(10)
            });
        }
    }
}
