using Microsoft.Data.SqlClient;
using MySqlConnector;

namespace  BankOfChocboy.Infrastructure
{
    public interface IDbConnectionFactory
    {
        MySqlConnection CreateConnection();
    }
}
