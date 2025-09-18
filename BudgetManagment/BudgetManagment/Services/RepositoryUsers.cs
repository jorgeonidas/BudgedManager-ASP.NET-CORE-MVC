using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BudgetManagment.Services
{
    public class RepositoryUsers : IRepositoryUsers
    {
        private readonly string _connectionString;
        public RepositoryUsers(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateUser(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var userId = await connection.QuerySingleAsync<int>(@"INSERT INTO Users (Email, NormalizedEmail, PasswordHash) 
                                                            VALUES (@Email, @NormalizedEmail, @PasswordHash)
                                                            SELECT SCOPE_IDENTITY();",
                                                            new { user.Email, user.NormalizedEmail, user.PasswordHash }
            );

            await connection.ExecuteAsync("CreateNewUserData", new { userId }, commandType: System.Data.CommandType.StoredProcedure );

            return userId;
        }

        public async Task<User> GetUserByEmail(string normalizedEmail)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                @"SELECT * FROM Users WHERE NormalizedEmail = @NormalizedEmail", 
                new { normalizedEmail }
            );
        }

        public async Task UpdateUserPassword(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                @"UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id",
                new { user.PasswordHash, user.Id }
            );
        }
    }
}
