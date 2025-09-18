using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "{0} is  required")]
        [EmailAddress(ErrorMessage = "Must be a valid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is  required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string  ResetCode { get; set; }
    }
}
