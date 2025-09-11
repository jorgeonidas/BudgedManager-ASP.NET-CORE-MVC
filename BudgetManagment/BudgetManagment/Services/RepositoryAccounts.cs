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

        public async Task<Account> GetAccountById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(
                @"SELECT Accounts.Id, Accounts.Name, Accounts.Balance, Accounts.Description, AccountTypeId
                  FROM Accounts
                  INNER JOIN AccountTypes ac
                  ON ac.Id = Accounts.AccountTypeId
                  WHERE ac.UserId = @UserId AND Accounts.Id = @Id", new { id, userId });
        }

        public async Task Update(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Accounts
                            SET Name = @Name, Balance = @Balance, Description = @Description,
                            AccountTypeId = @AccountTypeId
                            WHERE Id = @Id", account);
        }

        public async Task<int> Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteAsync("DELETE Accounts WHERE Id = @Id", new { id });
        }
    }
}
