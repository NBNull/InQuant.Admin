namespace InQuant.Authorization
{
    public class IRolePermission
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限项Name
        /// </summary>
        public string PermissionName { get; set; }
    }
}
