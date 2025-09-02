using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace BudgetManagment.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IRepositoryAccountTypes _repositoryAccountTypes;
        private readonly IUsersService _usersService;
        private readonly IRepositoryAccounts _repositoryAccounts;

        public AccountsController(IRepositoryAccountTypes repositoryAccountTypes, 
                                    IUsersService usersService, 
                                    IRepositoryAccounts repositoryAccounts)
        {
            this._repositoryAccountTypes = repositoryAccountTypes;
            this._usersService = usersService;
            this._repositoryAccounts = repositoryAccounts;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _usersService.GetUserId();
            var model = new AccountCreationViewModel();
            model.AccounTypes = await ObtainAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreationViewModel account)
        {
            var userId = _usersService.GetUserId();
            var accountType = await _repositoryAccountTypes.GetById(account.AccountTypeId, userId);
            if (accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            //In case of error we return to the creae account view but we have to get the account types frist
            if (!ModelState.IsValid)
            {
                account.AccounTypes = await ObtainAccountTypes(userId);
                return View(account);
            }

            await _repositoryAccounts.Create(account);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtainAccountTypes(int userId)
        {
            var accountTypes = await _repositoryAccountTypes.Obtain(userId);
            return accountTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
