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

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(_connectionString);
            //dapper code
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO AccountTypes (Name, UserId, Orden)
                                                    VALUES (@Name, @UserId, 0);
                                                    SELECT SCOPE_IDENTITY()", accountType);
            accountType.Id = id;
        }

        public async Task<bool> Exist(string name, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            //default int value is 0
            //query returns 1 if the data exist
            var exist = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 
                                                FROM AccountTypes 
                                                WHERE Name = @Name AND UserId = @UserId;",
                                                new { name, userId });
            return exist == 1;
        }


        /// <summary>
        /// Get accounts registered for this user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable of AccountType</returns>
        public async Task<IEnumerable<AccountType>> Obtain(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<AccountType>(@"SELECT Id, Name, Orden 
                                                        FROM AccountTypes 
                                                        WHERE UserId = @UserId", new { userId });
        }

        public async Task Update(AccountType accountType)
        {
            using var connection = new SqlConnection(_connectionString);
            //Executes a query with no return value
            await connection.ExecuteAsync(@"UPDATE AccountTypes
                                            SET Name = @Name
                                            WHERE Id = @Id", accountType);
        }

        public async Task<AccountType> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"SELECT Id, Name, Orden
                                        FROM AccountTypes
                                        WHERE Id = @Id AND UserId = @UserId
                                        ", new { id, userId });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"DELETE AccountTypes WHERE Id = @Id", new { id });
        } 
    }
}
