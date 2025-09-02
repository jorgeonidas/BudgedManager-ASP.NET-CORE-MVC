using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagment.Models
{
    public class AccountCreationViewModel : Account
    {
        public IEnumerable<SelectListItem> AccounTypes { get; set; }
    }
}
