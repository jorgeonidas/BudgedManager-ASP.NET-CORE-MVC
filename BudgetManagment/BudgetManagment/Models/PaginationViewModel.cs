namespace BudgetManagment.Models
{
    public class PaginationViewModel
    {
        public int Page { get; set; } = 1;
        private int recordsPerPager = 10;
        private readonly int maxRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPager;
            }
            set
            {
                recordsPerPager = value > maxRecordsPerPage ? maxRecordsPerPage : value;
            }
        }

        public int RecordsToJump => recordsPerPager * (Page - 1);
    }
}
