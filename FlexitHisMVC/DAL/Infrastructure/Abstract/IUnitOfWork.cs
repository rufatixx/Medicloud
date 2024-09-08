using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Infrastructure.Abstract
{
    public interface IUnitOfWork:IDisposable
    {
        MySqlTransaction BeginTransaction();
        MySqlConnection BeginConnection();
        MySqlConnection GetConnection();
        MySqlTransaction GetTransaction();
        void SaveChanges();
    }
}
