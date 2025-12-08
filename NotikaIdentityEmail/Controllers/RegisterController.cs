using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Services;

namespace NotikaIdentityEmail.Controllers
{
    public class RegisterController(UserManager<AppUser> userManager, IEmailService emailService) : Controller
    {

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            Random rnd = new();

            AppUser appUser = new()
            {
                FirstName = model.Firstname,
                LastName = model.Lastname,
                Email = model.Email,
                UserName = model.Username,
                ActivationCode = rnd.Next(100000, 1000000),
            };

            var result = await userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                await emailService.SendActivationMailAsync(appUser);
                TempData["EmailKey"] = model.Email;
                return RedirectToAction("UserActivation", "Activation");
            }
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
