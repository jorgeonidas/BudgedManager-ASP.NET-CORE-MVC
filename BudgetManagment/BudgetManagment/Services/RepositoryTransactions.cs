using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;
namespace BudgetManagment.Services
{
    public class RepositoryTransactions : IRepositoryTransactions
    {
        private readonly string _connectionString;
        public RepositoryTransactions(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>("Transactions_Insert",
                new
                {
                    transaction.UserId,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.Note,
                    transaction.AccountId
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaction.Id = id;
        }
    }
}
