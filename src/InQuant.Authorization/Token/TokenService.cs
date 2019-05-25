using InQuant.Cache;
using InQuant.Cache.Redis;
using InQuant.Authorization.Permissions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InQuant.Authorization.Token
{
    public class TokenService : ITokenService
    {
        private readonly AuthOption _securityOption;
        private readonly IDistributedCache _distributedCache;
        private readonly IRoleManager _roleManager;
        private readonly RedisOptions _redisOptions;

        private readonly string _cache_token = "token-{0}-{1}";//token-useId-guid

        public TokenService(IOptions<AuthOption> tokenOptions,
            IRoleManager roleManager,
            IOptions<RedisOptions> redisOptions,
            IDistributedCache distributedCache)
        {
            _securityOption = tokenOptions.Value;
            _roleManager = roleManager;
            _redisOptions = redisOptions.Value;
            _distributedCache = distributedCache;
        }

        public async Task DelToken(string tokenKey)
        {
            if (string.IsNullOrWhiteSpace(tokenKey))
                throw new ArgumentNullException(tokenKey);

            await _distributedCache.RemoveAsync(tokenKey);
        }

        public async Task<(string token, ClaimsPrincipal principal)> BuildToken(TokenInfo tokenData)
        {
            if (tokenData == null)
                throw new ArgumentNullException(nameof(tokenData));

            string token = string.Format(_cache_token, tokenData.UserId, Guid.NewGuid().ToString());

            TimeSpan expireIn = TimeSpan.Parse(_securityOption.TokenExpireTime);

            await _distributedCache.SetSlidingExpiration(token, tokenData, expireIn);//滑动过期，每次获取会自动延长过期时间

            var principal = await BuildPrincipal(token, tokenData);

            return (token, principal);
        }

        public async Task<ClaimsPrincipal> BuildPrincipal(string token, TokenInfo tokenData)
        {
            var claims = new List<Claim>()
                {
                    new Claim("token", token),
                    new Claim("isAdmin", tokenData.IsAdmin.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, tokenData.UserId.ToString()),
                    new Claim(ClaimTypes.Name,tokenData.UserName)
                };

            var permissions = await _roleManager.GetUserPermissions(tokenData.UserId);
            foreach (var p in permissions)
            {
                claims.Add(new Claim(Permission.ClaimType, p.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "TokenAuth");
            return new ClaimsPrincipal(new[] { claimsIdentity });
        }

        public async Task<TokenInfo> GetTokenInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            string json = await _distributedCache.GetStringAsync(token);

            if (string.IsNullOrWhiteSpace(json)) return null;

            return JsonConvert.DeserializeObject<TokenInfo>(json);
        }

        public Task<CookieOptions> BuildCookieOptions()
        {
            TimeSpan expire = TimeSpan.Parse(_securityOption.TokenExpireTime);
            var options = new CookieOptions()
            {
                Expires = DateTime.UtcNow.Add(expire),
                HttpOnly = true,
                Domain = _securityOption.CookieDomain,
                Path = "/",
                IsEssential = true,
                MaxAge = expire
            };

            return Task.FromResult(options);
        }

        public async Task DelToken(int userId)
        {
            //hopex.admin.token.1.*
            var keys = await RedisHelper.KeysAsync(pattern: $"{_redisOptions.KeyPreffix ?? string.Empty}{string.Format(_cache_token, userId, "*")}");

            if (keys.Length > 0)
            {
                await Task.WhenAll(keys.Select(key =>
                _distributedCache.RemoveAsync(key.ToString().Replace(_redisOptions.KeyPreffix, string.Empty))));
            }
        }
    }
}
