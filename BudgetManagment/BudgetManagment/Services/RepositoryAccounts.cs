using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagment.Services
{
    public class RepositoryAccounts : IRepositoryAccounts
    {
        private readonly string connectionString;
        public RepositoryAccounts(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO Accounts (Name, AccountTypeId, Description,Balance) 
                VALUES (@Name, @AccountTypeId, @Description, @Balance);
                SELECT SCOPE_IDENTITY();", account);
            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Search(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(
                @"SELECT Accounts.Id, Accounts.Name, Balance, ac.Name as AccountType
                FROM Accounts
                INNER JOIN AccountTypes ac
                ON ac.Id = Accounts.AccountTypeId
                WHERE ac.UserId = @UserId
                ORDER BY ac.Orden", new { userId });
        }
    }
}
