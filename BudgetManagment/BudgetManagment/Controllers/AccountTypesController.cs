using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

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

        // we use Index() to display a list of elements by convention
        public async Task<IActionResult> Index()
        {
            var userId = 1;
            var accounTypes = await _repositoryAccountTypes.Obtain(userId);
            return View(accounTypes);
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
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistingAccountType(string name)
        {
            var userId = 1;
            var alreadyExistAccountType = await _repositoryAccountTypes.Exist(name, userId);
            if (alreadyExistAccountType)
            {
                return Json($"El nombre {name} already exist");
            }
            //Account type is available, it will notify to remote fuction
            return Json(true);
        }
    }
}
