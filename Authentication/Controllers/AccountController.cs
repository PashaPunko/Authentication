using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Authentication.ViewModels;
using Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signin(SigninViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                User user = new User { Email = model.Email,
                                        UserName = model.Name,
                                        RegisterDate = DateTime.Now,
                                        LoginDate=DateTime.Now,
                                        Status= "Unblocked"
                };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(model.Name);
                if (user != null && user.Status.Equals("Blocked"))
                {
                    ModelState.AddModelError(string.Empty, "This user is blocked");
                    return View(model);
                }
                var result =
                    await _signInManager.PasswordSignInAsync(model.Name, model.Password,false, false);
                if (result.Succeeded)
                {
                    
                    user.LoginDate = DateTime.Now;
                    user.Status = "Unblocked";
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }
    }
}