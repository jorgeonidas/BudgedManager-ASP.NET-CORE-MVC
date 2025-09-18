using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Must be a valid Email")]
        public string Email { get; set; }
    }
}
