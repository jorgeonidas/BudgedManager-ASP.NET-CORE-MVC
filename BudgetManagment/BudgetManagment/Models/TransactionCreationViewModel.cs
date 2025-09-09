using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagment.Models
{
    /// <summary>
    /// ViewModel used for creating a new transaction in the Razor Pages UI.
    /// Inherits all properties from the Transaction model, such as Id, UserId, TransactionDate, Amount, CategoryId, Note, and AccountId.
    /// Adds two properties, Accounts and Categories, which provide lists of selectable items for account and category dropdowns in the UI.
    /// This class is designed to supply both the transaction data and the necessary selection lists for the creation form.
    /// </summary>
    public class TransactionCreationViewModel : Transaction
    {
        /// <summary>
        /// List of accounts to be displayed as options in a dropdown menu.
        /// </summary>
        public IEnumerable<SelectListItem> Accounts { get; set; }

        /// <summary>
        /// List of categories to be displayed as options in a dropdown menu.
        /// </summary>
        public IEnumerable<SelectListItem> Categories { get; set; }
        [Display(Name = "Operation Type")]

        public OperationType OperationTypeId { get; set; }
    }
}
