using BudgetManagment.Validations;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        //custom error messages
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Name of the account type")]// Display attribute to change the name of the field in the validation message
        [FirstCharacterCapitalized]//our custom validation attribute
        public string Name { get; set; }
        public int UserId { get; set; }
        public int Orden { get; set; }
    }
}
