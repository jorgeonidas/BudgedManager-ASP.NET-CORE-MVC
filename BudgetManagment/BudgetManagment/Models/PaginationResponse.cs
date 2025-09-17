namespace BudgetManagment.Models
{
    public class PaginationResponse
    {
        public int Page { get; set; } = 1;
        public int RecordsPerPage { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPagesAmount => (int)Math.Ceiling((double)TotalRecords/ RecordsPerPage);
        public string BaseUrl { get; set; }
    }

    public class PaginationResponse<T> : PaginationResponse
    {
        public IEnumerable<T> Elements { get; set; }
    }
}
