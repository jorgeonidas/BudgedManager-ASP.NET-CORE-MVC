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

        public async Task<IActionResult> Index(PaginationViewModel pagination)
        {
            var userId = _usersService.GetUserId();
            var categories = await _repositoryCategories.Get(userId, pagination);
            var totalCategories = await _repositoryCategories.Count(userId);
            var responseViewModel = new PaginationResponse<Category>
            {
                Elements = categories,
                Page = pagination.Page,
                RecordsPerPage = pagination.RecordsPerPage,
                TotalRecords = totalCategories,
                BaseUrl = "/categories"
            };
            return View(responseViewModel);
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

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _usersService.GetUserId();
            var category = await _repositoryCategories.GetById(id, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categoryToEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryToEdit);
            }
            var userId = _usersService.GetUserId();
            var category = await _repositoryCategories.GetById(categoryToEdit.Id, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            categoryToEdit.UserId = userId;
            await _repositoryCategories.Update(categoryToEdit);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _usersService.GetUserId();
            var category = await _repositoryCategories.GetById(id, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = _usersService.GetUserId();
            var category = await _repositoryCategories.GetById(id, userId);
            if (category is null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            await _repositoryCategories.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
