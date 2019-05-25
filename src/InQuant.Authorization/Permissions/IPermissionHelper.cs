using System.Threading.Tasks;

namespace InQuant.Authorization.Permissions
{
    public interface IPermissionHelper
    {
        Task<bool> HasPermission(string permissionName);
    }
}
