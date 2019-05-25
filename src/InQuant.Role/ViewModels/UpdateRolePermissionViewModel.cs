using System.ComponentModel.DataAnnotations;

namespace InQuant.Security.ViewModels
{
    public class UpdateRolePermissionViewModel
    {
        [Required(ErrorMessage ="角色ID不能为空")]
        public int RoleId { get; set; }
        
        public string[] PermissionNames { get; set; }
    }
}
