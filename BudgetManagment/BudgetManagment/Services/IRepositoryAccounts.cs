using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryAccounts
    {
        Task Create(Account account);
    }
}
