using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryCategories
    {
        Task Create(Category category);
        Task<IEnumerable<Category>> Get(int userId);
        Task<Category> GetById(int id, int userId);
        Task Update(Category category);
    }
}
