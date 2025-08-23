using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagment.Services
{
    public class RepositoryAccountTypes : IRepositoryAccountTypes
    {
        private readonly string _connectionString;
        public RepositoryAccountTypes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Create(AccountType accountType)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = connection.QuerySingle<int>($@"INSERT INTO AccountTypes (Name, UserId, Orden)
                                                    VALUES (@Name, @UserId, 0);
                                                    SELECT SCOPE_IDENTITY()", accountType);
            accountType.Id = id;
        }
    }
}
