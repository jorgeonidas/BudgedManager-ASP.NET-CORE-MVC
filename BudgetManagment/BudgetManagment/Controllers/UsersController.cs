using BudgetManagment.Models;
using BudgetManagment.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BudgetManagment.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public UsersController(UserManager<User> userManager, 
            SignInManager<User> signInManager, IEmailService emailService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                // Sign in the user if the registration is successful
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Transactions");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]    
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.Email, 
                model.Password, 
                isPersistent: model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Transactions");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Wrong Email or Password");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transactions");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var message = "If an account with the email you entered exists, you will receive instructions to reset your password shortly.";
            ViewBag.Message = message;
            ModelState.Clear();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var baseCode64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var link = Url.Action("RecoverPassword", "Users", new { code = baseCode64 }, protocol: Request.Scheme);
            await _emailService.SendEmailToChangePassword(model.Email, link);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecoverPassword(string code = null)
        {
            if (code is null)
            {
                var message = "Code not found";
                return RedirectToAction("ForgotPassword", new { message });
            }
            var model = new RecoverPasswordViewModel()
            {
                ResetCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)) 
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task <IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
            {
                return RedirectToAction("ChangedPassword");
            }
            var results = await _userManager.ResetPasswordAsync(user, model.ResetCode, model.Password);
            return RedirectToAction("ChangedPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangedPassword()
        {
            return View();
        }
    }
}
