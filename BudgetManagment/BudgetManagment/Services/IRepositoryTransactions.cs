using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryTransactions
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccounId(GetTransactionsByAccount model);
        Task<Transaction> GetById(int id, int userId);
        Task<IEnumerable<Transaction>> GetByUserId(TransasctionsPerUserParameters model);
        Task<IEnumerable<ResultByWeek>> GetResultsByWeek(TransasctionsPerUserParameters model);
        Task Update(Transaction transaction, decimal previousAmount, int previosAccount);
    }
}
