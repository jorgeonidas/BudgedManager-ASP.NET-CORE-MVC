using BudgetManagment.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BudgetManagment.Controllers
{
    public class AccountTypesController : Controller
    {
        private readonly string  _connectionString;
        public AccountTypesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Create()
        {
            //testing db connection
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = connection.Query("SELECT 1").FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(AccountType accountType)
        {
            //if the model state is not valid, return the same view with the model to show validation errors
            if (!ModelState.IsValid)
            {
                return View(accountType);
            }
            // Here you would typically save the new account type to the database
            return View();
        }
    }
}
