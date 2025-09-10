namespace BudgetManagment.Models
{
    public class GetTransactionsByAccount
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}
