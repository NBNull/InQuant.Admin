using InQuant.Authorization;
using InQuant.Framework.Data.Core;
using InQuant.Framework.Data.Core.Attributes;

namespace InQuant.Security.Models.Entities
{
    [TableName("t_role")]
    public class AdminRole : Role, IEntity
    {
    }

}
