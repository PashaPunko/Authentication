using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Authentication.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<HomeController> _logger;

        private async Task<bool> ValidateStatus() 
        {

            if (this.User.Identity.Name != null)
            {
                User user = await _userManager.FindByNameAsync(this.User.Identity.Name);
                if (user == null || user.Status.Equals("Blocked"))
                {
                    _signInManager.SignOutAsync().Wait();
                    return false;
                }
            }
            return true;
        }
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            if (!await ValidateStatus())
            {
                return RedirectToAction("Login", "Account");
            }
            if (User.Identity.IsAuthenticated)
            {
                return View(new Users { users = _userManager.Users.ToList() });
            }
            else return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Block(Users model)
        {
            if (!await ValidateStatus())
            {
                return RedirectToAction("Login", "Account");
            }
            ChangeStatus(model, "Blocked");
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Unblock(Users model)
        {
            if (!await ValidateStatus())
            {
                return RedirectToAction("Login", "Account");
            }
            ChangeStatus(model, "Unblocked");
            return RedirectToAction("Index", "Home");
        }
        private void ChangeStatus(Users model, string status)
        {
            foreach (var el in model.users)
            {
                if (el.IsChecked)
                {
                    User user =  _userManager.Users.Where(u=>u.Id == el.Id).FirstOrDefault();
                    user.Status = status;
                    _userManager.UpdateAsync(user).Wait();
                }
            };
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Users model)
        {
            if (!await ValidateStatus())
            {
                return RedirectToAction("Login", "Account");
            }
            foreach (var el in model.users)
            {
                if (el.IsChecked)
                {
                    User user = _userManager.Users.Where(u => u.Id == el.Id).FirstOrDefault();
                    _userManager.DeleteAsync(user).Wait();
                }
            };
            return RedirectToAction("Index", "Home");
        }

    }
}



 
