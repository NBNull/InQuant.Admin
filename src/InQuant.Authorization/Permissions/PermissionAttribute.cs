using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InQuant.Authorization.Permissions
{
    /// <summary>
    /// 授权Filter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly Permission _permission;

        public PermissionAttribute(string permissionName)
        {
            _permission = new Permission(permissionName);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, _permission);

            if (!authorizationResult)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
