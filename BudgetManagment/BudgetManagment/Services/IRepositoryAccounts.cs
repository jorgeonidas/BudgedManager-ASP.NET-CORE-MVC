using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryAccounts
    {
        Task Create(Account account);
        Task<int> Delete(int id);
        Task<Account> GetAccountById(int id, int userId);
        Task<IEnumerable<Account>> Search(int uderId);
        Task Update(Account account);
    }
}
