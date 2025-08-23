using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagment.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly IRepositoryAccountTypes _repositoryAccountTypes;

        public AccountTypesController(IRepositoryAccountTypes repositoryAccountTypes)
        {
            this._repositoryAccountTypes = repositoryAccountTypes;
        }

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

            //test user id
            accountType.UserId = 1;
            _repositoryAccountTypes.Create(accountType);

            // Here you would typically save the new account type to the database
            return View();
        }
    }
}
