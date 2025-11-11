using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController(SignInManager<AppUser> signInManager) : Controller
    {
        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
            if (result.Succeeded) return RedirectToAction("Profile", "MyProfile");
            else ModelState.AddModelError("", "Kullanıcı Adı yada Şifre Yanlış");
            return View(model);

        }


    }
}
