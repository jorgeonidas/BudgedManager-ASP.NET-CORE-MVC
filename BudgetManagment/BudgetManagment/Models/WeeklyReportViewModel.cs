namespace BudgetManagment.Models
{
    public class WeeklyReportViewModel
    {
        public decimal Income => TransactionsByWeek.Sum(x => x.Income);
        public decimal Expenses => TransactionsByWeek.Sum(x => x.Expense);
        public decimal Total => Income - Expenses;
        public DateTime DateOfReference { get; set; }
        public IEnumerable<ResultByWeek> TransactionsByWeek { get; set; }
    }
}
