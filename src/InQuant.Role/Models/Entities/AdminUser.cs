using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;
using System;

namespace InQuant.Security.Models.Entities
{
    [TableName("t_admin_user")]
    public class AdminUser : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// password hash salt
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// 加密后的密码
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 账号是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int LastModifier { get; set; }
    }
}
