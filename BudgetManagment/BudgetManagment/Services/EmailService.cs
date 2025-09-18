using System.Net;
using System.Net.Mail;

namespace BudgetManagment.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailToChangePassword(string receptor, string link)
        {
            var email = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:EMAIL");
            var password = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:PASSWORD");
            var host = configuration.GetValue<string>("CONFIGURATIONS_EMAIL:HOST");
            var port = configuration.GetValue<int>("CONFIGURATIONS_EMAIL:PORT");

            var client = new SmtpClient(host, port);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;

            client.Credentials = new NetworkCredential(email, password);
            var emmiter = email;
            var subject = "Forgot your password?";
            var htmlContent = $@"We received a request to reset the password for your Budget Management account.
                            If you made this request, please click the link below to reset your password:
                            {link}
                            Thank you,
                            The Budget Management Team";

            var message = new MailMessage(emmiter, receptor, subject, htmlContent);
            await client.SendMailAsync(message);
        }
    }
}
