using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryCategories
    {
        Task<int> Count(int userId);
        Task Create(Category category);
        Task<int> Delete(int id);
        Task<IEnumerable<Category>> Get(int userId, PaginationViewModel pagination);
        Task<IEnumerable<Category>> Get(int userId, OperationType operationTypeId);
        Task<Category> GetById(int id, int userId);
        Task Update(Category category);
    }
}
