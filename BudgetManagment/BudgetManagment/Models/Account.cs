using BudgetManagment.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(maximumLength: 50)]
        [FirstCharacterCapitalized]
        public string Name { get; set; }
        [Display(Name = "Account Type")]
        public  int AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Description { get; set; }
        public string AccountType { get; set; }
    }
}
