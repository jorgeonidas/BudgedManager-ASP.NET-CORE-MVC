using AutoMapper;
using BudgetManagment.Models;
using BudgetManagment.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

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

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = _usersService.GetUserId();
            IEnumerable<ResultByWeek> weeklyTransactions = 
                await _reportsService.GetWeelkyReport(userId, month, year, ViewBag);
            var grouped = weeklyTransactions
                .GroupBy(x => x.Week)
                .Select(x => new ResultByWeek()
                {
                    Week = x.Key,
                    Income = x.
                            Where(x => x.OperationTypeId == OperationType.Income).
                            Select(x => x.Amount)
                            .FirstOrDefault(),
                    Expense = x.
                                Where(x => x.OperationTypeId == OperationType.Expense).
                                Select(x => x.Amount).
                                FirstOrDefault(),

                }).ToList();

            if (year == 0 || month == 0)
            {
                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var referenceDate = new DateTime(year, month, 1);
            var daysOfMonth = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var segmentedDays = daysOfMonth.Chunk(7).ToList();

            for (int i = 0; i < segmentedDays.Count; i++)
            {
                var week = i + 1;
                var startDay = new DateTime(year, month, segmentedDays[i].First());
                var endDate = new DateTime(year, month, segmentedDays[i].Last());
                var weekGroup = grouped.FirstOrDefault(x => x.Week == week);

                if(weekGroup is null)
                {
                    grouped.Add(new ResultByWeek()
                    {
                        Week = week,
                        StartDate = startDay,
                        EndDate = endDate,
                    });
                }
                else
                {
                    weekGroup.StartDate = startDay;
                    weekGroup.EndDate = endDate;
                }
            }

            grouped = grouped.OrderByDescending(x => x.Week).ToList();
            var model = new WeeklyReportViewModel()
            {
                TransactionsByWeek = grouped,
                DateOfReference = referenceDate
            };

            return View(model);
        }

        public async Task<IActionResult> Monthly(int year)
        {
            var userId = _usersService.GetUserId();
            if(year == 0)
            {
                year = DateTime.Today.Year;
            }
            var transactionsPerMonth = await _repositoryTransactions.GetByMonth(userId, year);
            var groupedTransactions = transactionsPerMonth.GroupBy(x => x.Month).Select(x => new GetByMonthResult
            {
                Month = x.Key,
                Income = x.Where(x => x.OperationTypeId == OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                Expense = x.Where(x => x.OperationTypeId == OperationType.Expense).Select(x => x.Amount).FirstOrDefault(),
            })
            .ToList();

            for (int month = 1; month < 12; month++)
            {
                var transaction = groupedTransactions.FirstOrDefault(x => x.Month == month);
                var referenceDate = new DateTime(year, month, 1);
                if (transaction is null)
                {
                    groupedTransactions.Add(new GetByMonthResult()
                    {
                        Month = month,
                        ReferenceDate = referenceDate,
                    });
                }
                else
                {
                    transaction.ReferenceDate = referenceDate;
                }
            }
            groupedTransactions = groupedTransactions.OrderByDescending(x => x.Month).ToList();
            var model = new MonthlyReportViewModel()
            {
                MonthlyTransactions = groupedTransactions,
                Year = year
            };
            return View(model);
        }

        public IActionResult ExcelReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelPerMonth(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var userId = _usersService.GetUserId();
            var transactions = await _repositoryTransactions.GetByUserId(new TransasctionsPerUserParameters()
            {
                UserId = userId,
                StartDate = startDate,
                FinishDate = endDate
            });

            var fileName = $"Budge Managment - {startDate.ToString("MMM yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelPerYear(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            //last day of next year
            var endDate = startDate.AddYears(1).AddDays(-1);
            var userId = _usersService.GetUserId();
            var transactions = await _repositoryTransactions.GetByUserId(new TransasctionsPerUserParameters()
            {
                UserId = userId,
                StartDate = startDate,
                FinishDate = endDate
            });

            var fileName = $"Budge Managment - {startDate.ToString("yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelEverything()
        {
            var startDate = DateTime.Today.AddYears(-100);
            var endDate = DateTime.Today.AddYears(1000);

            var userId = _usersService.GetUserId();
            var transactions = await _repositoryTransactions.GetByUserId(new TransasctionsPerUserParameters()
            {
                UserId = userId,
                StartDate = startDate,
                FinishDate = endDate
            });
            var fileName = $"Budge Managment - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Transaction> transactions)
        {
            DataTable dataTable = new DataTable("Transactions");
            dataTable.Columns.AddRange(new DataColumn[] { 
                new DataColumn("Date"),
                new DataColumn("Account"),
                new DataColumn("Category"),
                new DataColumn("Note"),
                new DataColumn("Amount"),
                new DataColumn("Income/Outcome"),
            });

            foreach(var transaction in transactions)
            {
                dataTable.Rows.Add(transaction.TransactionDate.ToString("dd/MM/yyyy"),
                                    transaction.Account,
                                    transaction.Category,
                                    transaction.Note,
                                    transaction.Amount,
                                    transaction.OperationTypeId);
            }

            using(XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                fileName);
                }
            }
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetTransactionCalendar(DateTime start, DateTime end)
        {
            var userId = _usersService.GetUserId();
            var transactions = await _repositoryTransactions.GetByUserId(new TransasctionsPerUserParameters()
            {
                UserId = userId,
                StartDate = start,
                FinishDate = end
            });

            var calendarEvents = transactions.Select(x => new CalendarEvent()
            {
                Title = x.Amount.ToString("N") ,
                Start = x.TransactionDate.ToString("yyyy-MM-dd"),
                End = x.TransactionDate.ToString("yyyy-MM-dd"),
                Color = x.OperationTypeId == OperationType.Income ? null : "red"
            });

            return Json(calendarEvents);
        }

        public async Task<JsonResult> GetTransactionByDate(DateTime date)
        {
            var userId = _usersService.GetUserId();
            var transactions = await _repositoryTransactions.GetByUserId(new TransasctionsPerUserParameters()
            {
                UserId = userId,
                StartDate = date,
                FinishDate = date
            });
            return Json(transactions);
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
