using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryCategories
    {
        Task Create(Category category);
    }
}
