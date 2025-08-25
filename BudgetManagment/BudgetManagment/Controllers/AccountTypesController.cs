using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace BudgetManagment.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly IRepositoryAccountTypes _repositoryAccountTypes;
        private readonly IUsersService _usersService;

        public AccountTypesController(IRepositoryAccountTypes repositoryAccountTypes, IUsersService usersService)
        {
            this._repositoryAccountTypes = repositoryAccountTypes;
            this._usersService = usersService;
        }

        public IActionResult Create()
        {
            return View();
        }

        // we use Index() to display a list of elements by convention
        public async Task<IActionResult> Index()
        {
            var userId = _usersService.GetUserId();
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
            accountType.UserId = _usersService.GetUserId();

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
            var userId = _usersService.GetUserId();
            var alreadyExistAccountType = await _repositoryAccountTypes.Exist(name, userId);
            if (alreadyExistAccountType)
            {
                return Json($"El nombre {name} already exist");
            }
            //Account type is available, it will notify to remote fuction
            return Json(true);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var userId = _usersService.GetUserId();
            var accountType = await _repositoryAccountTypes.GetById(id, userId);

            if (accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AccountType accountType)
        {
            var userId = _usersService.GetUserId();
            var accountTypeExist = await _repositoryAccountTypes.GetById(accountType.Id, userId);

            if (accountTypeExist == null)
            {
                return RedirectToAction("NotFound", "Home");//NotFound Action from HomeController
            }
            await _repositoryAccountTypes.Update(accountType);
            return RedirectToAction("Index");
        }

        //returns delte view
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersService.GetUserId();
            var accountType = await _repositoryAccountTypes.GetById(id,userId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var userId = _usersService.GetUserId();
            var accountType = await _repositoryAccountTypes.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _repositoryAccountTypes.Delete(accountType.Id);

            return RedirectToAction("Index");
        }
    }
}
