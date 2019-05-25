using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InQuant.Authorization.Token
{
    public interface ITokenService
    {
        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="tokenData"></param>
        /// <returns></returns>
        Task<(string token, ClaimsPrincipal principal)> BuildToken(TokenInfo tokenData);

        /// <summary>
        /// 根据token生成principal
        /// </summary>
        /// <param name="tokenData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ClaimsPrincipal> BuildPrincipal(string token, TokenInfo tokenData);

        /// <summary>
        /// 获取token信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<TokenInfo> GetTokenInfo(string token);

        /// <summary>
        /// 删除用户的token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DelToken(string token);

        Task<CookieOptions> BuildCookieOptions();

        /// <summary>
        /// 删除用户所有的token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DelToken(int userId);
    }
}
