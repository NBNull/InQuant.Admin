using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace InQuant.Authorization.Permissions
{
    /// <summary>
    /// This authorization handler ensures that the user has the required permission.
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IOptions<AuthOption> _securityOption;
        public PermissionHandler(IOptions<AuthOption> tokenOptions)
        {
            _securityOption = tokenOptions;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!(bool)context?.User?.Identity?.IsAuthenticated)
            {
                return Task.CompletedTask;
            }
            else if (context.User.HasClaim(Permission.ClaimType, requirement.Permission.Name))
            {
                context.Succeed(requirement);
            }
            else if (bool.Parse(context.User.Claims.FirstOrDefault(x => x.Type == "isAdmin")?.Value ?? "False"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
