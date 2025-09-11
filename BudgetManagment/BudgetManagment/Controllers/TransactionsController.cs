using AutoMapper;
using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Threading.Tasks;

namespace BudgetManagment.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IRepositoryAccounts _repositoryAccounts;
        private readonly IRepositoryCategories _repositoryCategories;
        private readonly IRepositoryTransactions _repositoryTransactions;
        private readonly IMapper _mapper;
        private readonly IReportsService _reportsService;

        public TransactionsController(IUsersService usersService,
                                        IRepositoryAccounts repositoryAccounts,
                                        IRepositoryCategories repositoryCategories,
                                        IRepositoryTransactions repositoryTransactions, 
                                        IMapper mapper,
                                        IReportsService reportsService)
        {
            this._usersService = usersService;
            this._repositoryAccounts = repositoryAccounts;
            this._repositoryCategories = repositoryCategories;
            this._repositoryTransactions = repositoryTransactions;
            this._mapper = mapper;
            this._reportsService = reportsService;
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = _usersService.GetUserId();
            var model = await _reportsService.GetDetailedTransactionReport(userId, month, year, ViewBag);
            return View(model);
        }

        public IActionResult Weekly()
        {
            return View();
        }

        public IActionResult Monthly()
        {
            return View();
        }

        public IActionResult ExcelReport()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var userId = _usersService.GetUserId();
            var model = new TransactionCreationViewModel();
            model.Accounts = await GetAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreationViewModel model)
        {
            var userId = _usersService.GetUserId();
            if (!ModelState.IsValid)
            {
                //return to create view
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                model.Accounts = await GetAccounts(userId);
                return View(model);
            }
            //validate account
            var account = await _repositoryAccounts.GetAccountById(model.AccountId, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            //validate category
            var category = await _repositoryCategories.GetById(model.AccountId, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            model.UserId = userId;
            if (model.OperationTypeId == OperationType.Expense)
            {
                model.Amount *= -1;
            }

            await _repositoryTransactions.Create(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string returningUrl = null)
        {
            var userId = _usersService.GetUserId();
            var transaction = await _repositoryTransactions.GetById(id, userId);
            if (transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await _repositoryTransactions.Delete(id);
            if (!string.IsNullOrEmpty(returningUrl))
            {
                return LocalRedirect(returningUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetAccounts(int userId)
        {
            var accounts = await _repositoryAccounts.Search(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string returningUrl = null)
        {
            var userId = _usersService.GetUserId();
            var transaction = await _repositoryTransactions.GetById(id, userId);
            if (transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var model = _mapper.Map<TransactionUpdateViewModel>(transaction);
            model.PreviousAmount = model.Amount;
            if (model.OperationTypeId == OperationType.Expense)
            {
                model.Amount = model.Amount * -1;
            }
            model.PreviousAccountId = transaction.AccountId;
            model.Categories = await GetCategories(userId, transaction.OperationTypeId);
            model.Accounts = await GetAccounts(userId);
            model.ReturningUrl = returningUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TransactionUpdateViewModel model)
        {
            var userId = _usersService.GetUserId();
            //validate transaction, if not valid return to de Edit view
            if (!ModelState.IsValid)
            {
                model.Accounts = await GetAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await _repositoryAccounts.GetAccountById(model.AccountId, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var category = await _repositoryCategories.GetById(model.CategoryId, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = _mapper.Map<Transaction>(model);
            if (model.OperationTypeId == OperationType.Expense)
            {
                transaction.Amount = model.Amount * -1;
            }

            await _repositoryTransactions.Update(transaction, model.PreviousAmount, model.PreviousAccountId);
            if (!string.IsNullOrEmpty(model.ReturningUrl))
            {
                return LocalRedirect(model.ReturningUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
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
