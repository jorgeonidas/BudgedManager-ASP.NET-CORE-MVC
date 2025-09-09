using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaction transaction);
    }
}
