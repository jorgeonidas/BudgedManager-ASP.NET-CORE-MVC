using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetManagment.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepositoryCategories _repositoryCategories;
        private readonly IUsersService _usersService;

        public CategoriesController(IRepositoryCategories repositoryCategories, IUsersService usersService)
        {
            this._repositoryCategories = repositoryCategories;
            this._usersService = usersService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            category.UserId = _usersService.GetUserId();
            await _repositoryCategories.Create(category);
            return RedirectToAction("Index");
        }   
    }
}
