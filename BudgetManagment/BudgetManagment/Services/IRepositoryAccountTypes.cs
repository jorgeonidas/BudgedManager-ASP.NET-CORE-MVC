using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryAccountTypes
    {
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<bool> Exist(string name, int userId);
        Task<AccountType> GetById(int id, int userId);
        Task<IEnumerable<AccountType>> Obtain(int userId);
        Task Order(IEnumerable<AccountType> accountTypesOrdered);
        Task Update(AccountType accountType);
    }
}
