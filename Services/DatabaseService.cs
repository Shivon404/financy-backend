using MySql.Data.MySqlClient;

namespace FinancyAPI.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Server=localhost;Database=financy_db;Uid=root;Pwd=;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}