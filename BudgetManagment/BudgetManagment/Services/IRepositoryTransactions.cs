using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<Transaction> GetById(int id, int userId);
        Task Update(Transaction transaction, decimal previousAmount, int previosAccount);
    }
}
