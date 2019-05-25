namespace InQuant.Framework.Data.Core.Sessions
{
    public interface IConnectionStringProvider
    {
        string ConnectionString(string connectionStringName);
    }
}
