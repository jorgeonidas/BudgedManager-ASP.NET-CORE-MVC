using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IRepositoryTransactions _repositoryTransactions;

        public AccountsController(IRepositoryAccountTypes repositoryAccountTypes, 
                                    IUsersService usersService, 
                                    IRepositoryAccounts repositoryAccounts,
                                    IMapper mapper,
                                    IRepositoryTransactions repositoryTransactions)
        {
            this._repositoryAccountTypes = repositoryAccountTypes;
            this._usersService = usersService;
            this._repositoryAccounts = repositoryAccounts;
            this._mapper = mapper;
            this._repositoryTransactions = repositoryTransactions;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _usersService.GetUserId();
            var accountsWithAccountType = await _repositoryAccounts.Search(userId);

            var model = accountsWithAccountType
                        .GroupBy(x => x.AccountType)
                        .Select(group => new AccountIndexViewModel{
                            AccountType = group.Key,
                            Accounts = group.AsEnumerable()
                        }).ToList();

            return View(model);
        }

        public async Task <IActionResult> Details(int id, int month, int year)
        {
            var userId = _usersService.GetUserId();
            var account = await _repositoryAccounts.GetAccountById(id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            DateTime startDate;
            DateTime finishDate;

            if(month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                startDate = new DateTime(year, month, 1);
            }
            //We want the last day of the month
            finishDate = startDate.AddMonths(1).AddDays(-1);

            var getTransasctionsByAccoun = new GetTransactionsByAccount()
            {
                AccountId = id,
                UserId = userId,
                StartDate = startDate,
                FinishDate = finishDate
            };

            var transactions = await _repositoryTransactions.GetByAccounId(getTransasctionsByAccoun);
            var model = new DetailedTransactionReport();
            ViewBag.Account = account.Name;

            var transactiosnPerDate = transactions.OrderByDescending(x => x.TransactionDate)
                                                 .GroupBy(x => x.TransactionDate)
                                                 .Select(group => new DetailedTransactionReport.TransactionPerDate()
                                                 {
                                                     TransactionDate = group.Key,
                                                     Transactions = group.AsEnumerable()
                                                 });

            model.GroupedTransactions = transactiosnPerDate;
            model.StartDate = startDate;
            model.FinishDate = finishDate;

            ViewBag.previousMonth = startDate.AddMonths(-1).Month;
            ViewBag.previousYear = startDate.AddMonths(-1).Year;
            ViewBag.followingMonth = startDate.AddMonths(1).Month;
            ViewBag.followingYear = startDate.AddMonths(1).Year ;

            return View(model);
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

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _usersService.GetUserId();
            var account = await _repositoryAccounts.GetAccountById(id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            //OLD: Map the account to the view model
           /* var model = new AccountCreationViewModel()
            {
                Id = account.Id,
                Name = account.Name,
                AccountType = account.AccountType,
                Balance = account.Balance,
                Description = account.Description,
            };*/
           //using auto mapper
            var model = _mapper.Map<AccountCreationViewModel>(account);
            model.AccounTypes = await ObtainAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountCreationViewModel editAccount)
        {
            var userId = _usersService.GetUserId();
            var account = await _repositoryAccounts.GetAccountById(editAccount.Id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            var accountType = await _repositoryAccountTypes.GetById(account.AccountTypeId, userId);
            if (accountType == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _repositoryAccounts.Update(editAccount);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersService.GetUserId();
            var account = await _repositoryAccounts.GetAccountById(id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = _usersService.GetUserId();
            var account = await _repositoryAccounts.GetAccountById(id, userId);
            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await _repositoryAccounts.Delete(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtainAccountTypes(int userId)
        {
            var accountTypes = await _repositoryAccountTypes.Obtain(userId);
            return accountTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
