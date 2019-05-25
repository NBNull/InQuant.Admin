using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace InQuant.Authorization.Permissions
{
    public class PermissionHelper : IPermissionHelper
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHelper(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> HasPermission(string permissionName)
        {
            return await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, new Permission(permissionName));
        }
    }
}
