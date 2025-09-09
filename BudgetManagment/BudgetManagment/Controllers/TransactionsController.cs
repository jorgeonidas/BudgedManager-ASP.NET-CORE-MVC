using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BudgetManagment.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryAccounts _repositoryAccounts;

        public TransactionsController(IUsersService usersService, IRepositoryAccounts repositoryAccounts)
        {
            this._usersService = usersService;
            this._repositoryAccounts = repositoryAccounts;
        }

        public async Task<IActionResult> Create()
        {
            var userId = _usersService.GetUserId();
            var model = new TransactionCreationViewModel();
            model.Accounts = await GetAccounts(userId);
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        {
            var accounts = await _repositoryAccounts.Search(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
