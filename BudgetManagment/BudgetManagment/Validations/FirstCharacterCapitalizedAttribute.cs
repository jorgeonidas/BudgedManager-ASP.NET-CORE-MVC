using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Validations
{
    public class FirstCharacterCapitalizedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null or empty, we consider it valid (use [Required] for mandatory fields)
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var firstCharacter = value.ToString()[0].ToString();
            if(firstCharacter != firstCharacter.ToUpper())
            {
                return new ValidationResult("The first character must be uppercase");
            }

            return ValidationResult.Success;
        }
    }
}
