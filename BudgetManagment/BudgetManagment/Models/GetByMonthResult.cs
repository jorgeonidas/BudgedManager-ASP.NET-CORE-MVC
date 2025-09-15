namespace BudgetManagment.Models
{
    public class GetByMonthResult
    {
        public int Month { get; set; }
        public DateTime ReferenceDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public  OperationType OperationTypeId { get; set; }
    }
}
