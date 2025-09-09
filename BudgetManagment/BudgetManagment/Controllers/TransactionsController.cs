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
        private readonly IRepositoryCategories _repositoryCategories;

        public TransactionsController(IUsersService usersService, 
                                        IRepositoryAccounts repositoryAccounts,
                                        IRepositoryCategories repositoryCategories)
        {
            this._usersService = usersService;
            this._repositoryAccounts = repositoryAccounts;
            this._repositoryCategories = repositoryCategories;
        }

        public async Task<IActionResult> Create()
        {
            var userId = _usersService.GetUserId();
            var model = new TransactionCreationViewModel();
            model.Accounts = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        {
            var accounts = await _repositoryAccounts.Search(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, 
            OperationType operationType)
        {
            var categories = await _repositoryCategories.Get(userId, operationType);
            return categories.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> GetCategories([FromBody] OperationType operationType)
        {
            var userId = _usersService.GetUserId();
            var categories = await GetCategories(userId, operationType);
            return Ok(categories);//Ok is success status code (200)

        }
    }
}
