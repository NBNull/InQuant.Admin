using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;
using InQuant.Framework.Data.Mapper;
using Microsoft.Extensions.Logging;

namespace InQuant.Security.Models.Entities
{
    [TableName("t_role_permission")]
    public class RolePermission : IEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限项名称
        /// </summary>
        public string PermissionName { get; set; }
    }    
}
