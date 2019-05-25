using System.Collections.Generic;

namespace InQuant.Security.Models
{
    public class UserRoleModel
    {
        public int UserId { get; set; }

        /// <summary>
        /// 用户所属角色ID
        /// </summary>
        public List<int> RoleIds { get; set; }

        /// <summary>
        /// 用户所属角色
        /// </summary>
        public string Roles { get; set; }
    }
}
