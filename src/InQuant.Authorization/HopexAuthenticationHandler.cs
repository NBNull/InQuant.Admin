using InQuant.Authorization.Token;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace InQuant.Authorization
{
    /// <summary>
    /// 认证，登入，退出服务
    /// </summary>
    public class HopexAuthenticationHandler : IAuthenticationHandler, IAuthenticationSignInHandler, IAuthenticationSignOutHandler
    {
        public const string _cookie_name = "admin_access_token";

        private readonly ITokenService _tokenService;
        private readonly ILogger<HopexAuthenticationHandler> _logger;
        protected HttpContext Context { get; private set; }

        public AuthenticationScheme Scheme { get; private set; }

        public HopexAuthenticationHandler(ITokenService tokenService, ILogger<HopexAuthenticationHandler> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Scheme = scheme;
            Context = context;
            return Task.CompletedTask;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            string token = Context.Request.Cookies[_cookie_name];
            var (ok, tokenInfo) = await TokenIsValid(token);

            if (!ok)
            {
                token = Context.Request.Headers["Authorization"];
                (ok, tokenInfo) = await TokenIsValid(token);

                if (!ok)
                {
                    return AuthenticateResult.NoResult();
                }
            }

            var principal = await _tokenService.BuildPrincipal(token, tokenInfo);
            await RefreshCookie(token, tokenInfo);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, HopexAuthenticationDefaults.AuthenticationScheme));
        }

        private async Task RefreshCookie(string token, TokenInfo tokenInfo)
        {
            Context.Response.Cookies.Append(_cookie_name, token, await _tokenService.BuildCookieOptions());
        }

        /// <summary>
        /// user.232.web
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<(bool ok, TokenInfo tokenInfo)> TokenIsValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, null);

            var tokenInfo = await _tokenService.GetTokenInfo(token);

            if (tokenInfo != null)
            {
                return (true, tokenInfo);
            }
            return (false, null);
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            if (Context.Request.Path.Value.Contains("/api/"))
            {
                Context.Response.StatusCode = 401;
            }
            else
            {
                string url = $"{Context.Request.Path}{Context.Request.QueryString}";
                Context.Response.Redirect($"{HopexAuthenticationDefaults.LoginPath}?returnUrl={HttpUtility.UrlEncode(url)}");
            }
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            if (Context.Request.Path.Value.Contains("/api/"))
            {
                Context.Response.StatusCode = 403;
            }
            else
            {
                Context.Response.Redirect(HopexAuthenticationDefaults.AccessDeniedPath);
            }
            return Task.CompletedTask;
        }

        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            string token = user.Claims.FirstOrDefault(x => x.Type == "token")?.Value;
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            Context.Response.Headers["Authorization"] = token;
            Context.Response.Cookies.Append(_cookie_name, token,await _tokenService.BuildCookieOptions());
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            Context.Response.Headers.Remove("Authorization");
            Context.Response.Cookies.Delete(_cookie_name);

            return Task.CompletedTask;
        }
    }

}