using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;

namespace InQuant.Security.Models.Entities
{
    /// <summary>
    /// 用户角色表
    /// </summary>
    [TableName("t_user_role")]
    public class UserRole : IEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public int RoleId { get; set; }
    }

}
