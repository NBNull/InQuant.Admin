using System.Collections.Generic;

namespace InQuant.Security.ViewModels
{
    public class UpdateAdminUserViewModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public IEnumerable<int> RoleIds { get; set; }
    }
}
