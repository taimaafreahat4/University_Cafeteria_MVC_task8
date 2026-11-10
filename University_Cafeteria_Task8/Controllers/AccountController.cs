using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using University_Cafeteria_Task8.Models;

namespace University_Cafeteria_Task8.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDTO register)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = register.Email,
                    Email = register.Email
                };

                var result = _userManager
                    .CreateAsync(user, register.Password)
                    .GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "User")
                        .GetAwaiter().GetResult();

                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(register);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDTO login)
        {
            if (ModelState.IsValid)
            {
                var result = _signInManager.PasswordSignInAsync(
                    login.Email,
                    login.Password,
                    false,
                    false
                ).GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(
                    "",
                    "Invalid email or password."
                );
            }

            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync()
                .GetAwaiter().GetResult();

            return RedirectToAction("Login");
        }

    }
}