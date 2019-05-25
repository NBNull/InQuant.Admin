using InQuant.Framework.Modules;
using System.Collections.Generic;

namespace InQuant.Authorization.Permissions
{
    /// <summary>
    /// Implemented by modules to enumerate the types of permissions
    /// the which may be granted
    /// </summary>
    public interface IPermissionProvider : ISingleton
    {
        IEnumerable<Permission> GetPermissions();
        IEnumerable<PermissionStereotype> GetDefaultStereotypes();
    }

    public class PermissionStereotype
    {
        public string Name { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
