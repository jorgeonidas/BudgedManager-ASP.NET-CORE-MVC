using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IReportsService
    {
        Task<DetailedTransactionReport> GetDetailedTransactionReport(int userId, int month, int year, dynamic viewBag);
        Task<DetailedTransactionReport> GetDetailedTransactionReportByAccount(int userId, int accountId, int month, int year, dynamic viewBag);
        Task<IEnumerable<ResultByWeek>> GetWeelkyReport(int userId, int month, int year, dynamic viewBag);
    }
}
