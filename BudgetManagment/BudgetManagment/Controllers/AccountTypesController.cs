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
        public async Task<IActionResult> Create(AccountType accountType)
        {
            //if the model state is not valid, return the same view with the model to show validation errors
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }

            //test user id
            accountType.UserId = 1;

            //check if the account type already exist for this userid
            var alreadyExist = await _repositoryAccountTypes.Exist(accountType.Name, accountType.UserId);
            if (alreadyExist)
            {
                ModelState.AddModelError(nameof(accountType.Name), 
                    $"Name {accountType.Name} already exist");

                return View(accountType);
            }
            
            await _repositoryAccountTypes.Create(accountType);

            // Here you would typically save the new account type to the database
            return View();
        }
    }
}
