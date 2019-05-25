using InQuant.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;

namespace InQuant.Authorization.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Permission Permission { get; set; }
    }
}
