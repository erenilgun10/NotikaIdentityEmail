using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController(SignInManager<AppUser> signInManager, EmailContext context) : Controller
    {
        private readonly EmailContext _context = context;

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            var usr = _context.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (usr == null)
            {
                ModelState.AddModelError("", "Kullanıcı Adı yada Şifre Yanlış");
                return View(model);
            }
            else if (usr.EmailConfirmed)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.Succeeded) return RedirectToAction("EditProfile","Profile");
                else ModelState.AddModelError("", "Kullanıcı Adı yada Şifre Yanlış");

            }
            else
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, true);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Giriş Bilgileriniz yanlıştır.");
                    return View();
                }
                ModelState.AddModelError("", "Kullanıcı Adı Henüz Onaylı Değildir. Onaylayınız.");
                return RedirectToAction("UserActivation", "Activation");
            }

            return View(model);

        }


    }
}
