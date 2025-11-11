using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class RegisterController(UserManager<AppUser> userManager) : Controller 
    {
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            AppUser appUser = new()
            {
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Email = model.Email,
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded) return RedirectToAction("UserLogin", "Login");
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View();
        }



    }
}
