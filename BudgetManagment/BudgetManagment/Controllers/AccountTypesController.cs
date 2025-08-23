using BudgetManagment.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagment.Controllers
{
    public class AccountTypesController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AccountType accountType)
        {
            //if the model state is not valid, return the same view with the model to show validation errors
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }
            // Here you would typically save the new account type to the database
            return View();
        }
    }
}
