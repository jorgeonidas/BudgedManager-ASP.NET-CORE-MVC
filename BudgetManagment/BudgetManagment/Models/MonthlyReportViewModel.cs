namespace BudgetManagment.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<GetByMonthResult> MonthlyTransactions { get; set; }
        public decimal Income => MonthlyTransactions.Sum(x => x.Income);
        public decimal Expenses => MonthlyTransactions.Sum(x => x.Expense);
        public decimal Total => Income - Expenses;
        public int Year { get; set; }
    }
}
