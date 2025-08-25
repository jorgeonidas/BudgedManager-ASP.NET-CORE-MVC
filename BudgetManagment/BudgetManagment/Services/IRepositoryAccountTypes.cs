using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryAccountTypes
    {
        Task Create(AccountType accountType);
        Task<bool> Exist(string name, int userId);
        Task<AccountType> GetById(int id, int userId);
        Task<IEnumerable<AccountType>> Obtain(int userId);
        Task Update(AccountType accountType);
    }
}
