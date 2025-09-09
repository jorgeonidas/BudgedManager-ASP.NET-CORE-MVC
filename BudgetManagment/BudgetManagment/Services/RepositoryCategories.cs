using BudgetManagment.Models;
using Dapper;
using Microsoft.Data.SqlClient;


namespace BudgetManagment.Services
{
    public class RepositoryCategories : IRepositoryCategories
    {
        private readonly string _connectionString;
        public RepositoryCategories(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categories (Name, OperationTypeId, UserId)
                                                            Values (@Name, @OperationTypeId, @UserId)
                                                            SELECT SCOPE_IDENTITY()
                                                            ", category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> Get(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories WHERE UserId = @userId", new { userId });
        }

        public async Task<Category> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(
                @"SELECT * FROM Categories WHERE Id = @Id AND UserId = @UserId", 
                new { id, userId }
            );
        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories 
                                            SET Name = @Name, OperationTypeId = @OperationTypeId
                                            WHERE Id = @Id", category);
        }

        public async Task<int> Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@"DELETE Categories WHERE Id = @Id", new { id });
        }
    }
}
