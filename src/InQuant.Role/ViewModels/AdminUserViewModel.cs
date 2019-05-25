using InQuant.Security.Models.Entities;
using System;
using System.Collections.Generic;

namespace InQuant.Users.ViewModels
{
    public class AdminUserViewModel
    {
        public AdminUserViewModel() { }
        public AdminUserViewModel(AdminUser r)
        {
            Id = r.Id;
            UserName = r.UserName;
            NickName = r.NickName;
            IsAdmin = r.IsAdmin;
            LastModifier = r.LastModifier;
            LastModifiedTime = r.LastModifiedTime;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string RoleName { get; set; }

        public bool IsAdmin { get; set; }

        public string IsAdminDisplay { get { return IsAdmin ? "是" : "否"; } }

        /// <summary>
        /// 角色ID
        /// </summary>
        public IEnumerable<int> RoleIds { get; set; }

        public int LastModifier { get; set; }

        public string LastModifierName { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}
