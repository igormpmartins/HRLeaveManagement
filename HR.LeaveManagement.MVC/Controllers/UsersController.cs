using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR.LeaveManagement.MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAuthenticationService authenticationService;

        public UsersController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public IActionResult Index(string returnUrl = null)
        {
            return View();
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }        

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string returnUrl)
        {
            ModelState.Remove(nameof(LoginVM.ReturnUrl));
            if (ModelState.IsValid)
            {
                returnUrl ??= Url.Content("~/");

                var isLoggedIn = await authenticationService.Authenticate(login.Email, login.Password);
                if (isLoggedIn)
                    return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError("", "Log in attempt failed. Please, try again.");
            return View(login);
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registration)
        {
            if (ModelState.IsValid)
            {
                var returnUrl = Url.Content("~/");
                var isCreated = await authenticationService.Register(registration);
                if (isCreated)
                    return LocalRedirect(returnUrl);

            }

            ModelState.AddModelError("", "Registration attempt failed. Please, try again.");
            return View(registration);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            returnUrl ??= Url.Content("~/");
            await authenticationService.Logout();
            return LocalRedirect(returnUrl);
        }
    }
}
