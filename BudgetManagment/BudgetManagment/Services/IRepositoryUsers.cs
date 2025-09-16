using BudgetManagment.Models;

namespace BudgetManagment.Services
{
    public interface IRepositoryUsers
    {
        Task<int> CreateUser(User user);
        Task<User> GetUserByEmail(string normalizedEmail);
    }
}
