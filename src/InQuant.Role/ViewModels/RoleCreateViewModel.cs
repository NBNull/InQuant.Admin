namespace InQuant.Security.ViewModels
{
    public class RoleCreateViewModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        public string[] PermissionNames { get; set; }
    }
}
