namespace InQuant.Security.ViewModels
{
    public class RoleUpdateViewModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        public string[] PermissionNames { get; set; }
    }
}
