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

        }
    }
}
