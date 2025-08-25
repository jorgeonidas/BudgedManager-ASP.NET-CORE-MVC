using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryAccountTypes
    {
        Task Create(AccountType accountType);
        Task<bool> Exist(string name, int userId);
    }
}
