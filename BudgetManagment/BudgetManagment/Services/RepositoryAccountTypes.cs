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
            //launch Store procedure from SQL MANAGMENT STUDIO
            var id = await connection.QuerySingleAsync<int>("AccountTypes_Insert",
                new { UserId = accountType.UserId, Name = accountType.Name }, commandType: System.Data.CommandType.StoredProcedure);
            accountType.Id = id;
        }

        public async Task<bool> Exist(string name, int userId, int id = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            //default int value is 0
            //query returns 1 if the data exist
            var exist = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 
                                                FROM AccountTypes 
                                                WHERE Name = @Name AND UserId = @UserId AND Id <> @id;",
                                                new { name, userId, id });
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
                                                        WHERE UserId = @UserId
                                                        ORDER BY Orden", new { userId });
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

        public async Task Order(IEnumerable<AccountType> accountTypesOrdered)
        {
            var query = "UPDATE AccountTypes SET Orden = @Orden where Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, accountTypesOrdered);
        }
    }
}
