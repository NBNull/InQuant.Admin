using System;
using System.Linq;
using System.Security.Claims;

namespace InQuant.Authorization.Token
{
    public static class ClaimPrincipalExtensions
    {
        public static string GetToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.Claims.FirstOrDefault(x => x.Type == "token")?.Value ?? string.Empty;
        }

        public static int GetId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var identity = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Convert.ToInt32(identity?.Value);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }
}
