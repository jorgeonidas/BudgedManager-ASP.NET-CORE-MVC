
namespace BudgetManagment.Services
{
    public interface IEmailService
    {
        Task SendEmailToChangePassword(string receptor, string link);
    }
}