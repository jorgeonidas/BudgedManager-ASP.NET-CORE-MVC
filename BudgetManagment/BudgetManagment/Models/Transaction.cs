using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        [Range(1,maximum: int.MaxValue, ErrorMessage = "Must select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "The note must be less than {1} characters")]
        public string Note { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Must select a account")]
        [Display(Name = "Account")]
        public int AccountId { get; set; }
    }
}
