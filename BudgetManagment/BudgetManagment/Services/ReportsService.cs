using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IRepositoryTransactions _repositoryTransactions;
        private readonly HttpContext _httpContext;

        public ReportsService(IRepositoryTransactions repositoryTransactions, IHttpContextAccessor httpContextAccessor)
        {
            this._repositoryTransactions = repositoryTransactions;
            this._httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultByWeek>> GetWeelkyReport(int userId, int month, int year, dynamic viewBag)
        {
            (DateTime startDate, DateTime finishDate) = GenerateStartAndFinishDate(month, year);
            var parameter = new TransasctionsPerUserParameters
            {
                UserId = userId,
                StartDate = startDate,
                FinishDate = finishDate
            };
            FillViewBag(viewBag, startDate);
            var model = await _repositoryTransactions.GetResultsByWeek(parameter);
            return model;
        }

        public async Task<DetailedTransactionReport>
             GetDetailedTransactionReport(int userId, int month, int year, dynamic viewBag)
        {
            (DateTime startDate, DateTime finishDate) = GenerateStartAndFinishDate(month, year);
            var parameter = new TransasctionsPerUserParameters
            {
                UserId = userId,
                StartDate = startDate,
                FinishDate = finishDate
            };
            var transactions = await _repositoryTransactions.GetByUserId(parameter);
            var model = GenerateDetailedTransactionReport(startDate, finishDate, transactions);
            FillViewBag(viewBag, startDate);
            return model;
        }

        public async Task<DetailedTransactionReport>
            GetDetailedTransactionReportByAccount(int userId,
                                        int accountId,
                                        int month,
                                        int year,
                                        dynamic viewBag)
        {
            (DateTime startDate, DateTime finishDate) = GenerateStartAndFinishDate(month, year);
            var getTransasctionsByAccoun = new GetTransactionsByAccount()
            {
                AccountId = accountId,
                UserId = userId,
                StartDate = startDate,
                FinishDate = finishDate
            };
            var transactions = await _repositoryTransactions.GetByAccounId(getTransasctionsByAccoun);

            var model = GenerateDetailedTransactionReport(startDate, finishDate, transactions);

            FillViewBag(viewBag, startDate);
            //To return to the same page after creating a new transaction
            viewBag.returningUrl = _httpContext.Request.Path + _httpContext.Request.QueryString;
            return model;
        }

        private static void FillViewBag(dynamic viewBag, DateTime startDate)
        {
            viewBag.previousMonth = startDate.AddMonths(-1).Month;
            viewBag.previousYear = startDate.AddMonths(-1).Year;
            viewBag.followingMonth = startDate.AddMonths(1).Month;
            viewBag.followingYear = startDate.AddMonths(1).Year;
        }

        private static DetailedTransactionReport GenerateDetailedTransactionReport(DateTime startDate, DateTime finishDate, IEnumerable<Transaction> transactions)
        {
            var model = new DetailedTransactionReport();

            var transactiosnPerDate = transactions.OrderByDescending(x => x.TransactionDate)
                                                 .GroupBy(x => x.TransactionDate)
                                                 .Select(group => new DetailedTransactionReport.TransactionPerDate()
                                                 {
                                                     TransactionDate = group.Key,
                                                     Transactions = group.AsEnumerable()
                                                 });

            model.GroupedTransactions = transactiosnPerDate;
            model.StartDate = startDate;
            model.FinishDate = finishDate;
            return model;
        }

        private (DateTime startDate, DateTime finishDate) GenerateStartAndFinishDate(int month, int year)
        {
            DateTime startDate;
            DateTime finishDate;
            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                startDate = new DateTime(year, month, 1);
            }
            //We want the last day of the month
            finishDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, finishDate);
        }
    }
}
