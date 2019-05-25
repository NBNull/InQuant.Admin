using System.Data;

namespace InQuant.Framework.Data.Core.Sessions
{
    public interface IDapperSession : IDbConnection
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
    }
}
