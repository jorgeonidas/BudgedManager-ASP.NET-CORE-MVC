using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(maximumLength: 50, ErrorMessage = "Cannot be longer than {0} characters")]
        public string Name { get; set; }
        [Display(Name = "Operation Type")]
        public int OperationTypeId { get; set; }
        public int UserId { get; set; }
    }
}
