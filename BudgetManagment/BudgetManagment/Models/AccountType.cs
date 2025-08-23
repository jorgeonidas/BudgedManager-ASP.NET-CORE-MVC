using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        //custom error messages
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} characters")]
        [Display(Name = "Name of the account type")]// Display attribute to change the name of the field in the validation message
        public string Name { get; set; }
        public int UserId { get; set; }
        public int Orden { get; set; }

        /*Other validation types*/
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "The field {0} is not a valid email address")]
        public string Email { get; set; }
        [Range(minimum: 18, maximum: 130, ErrorMessage = "The field {0} must be between {1} and {2}")]
        public int Age { get; set; }
        [Url(ErrorMessage = "The field {0} must be a valid URL")]
        public string Url { get; set; }
        [CreditCard(ErrorMessage = "The field {0} must be a valid credit card number")]
        [Display(Name = "Credit Card")]
        public string CreditCard { get; set; }
    }
}
