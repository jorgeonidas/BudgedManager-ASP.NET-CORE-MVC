namespace BudgetManagment.Models
{
    public class TransasctionsPerUserParameters
    {
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}
