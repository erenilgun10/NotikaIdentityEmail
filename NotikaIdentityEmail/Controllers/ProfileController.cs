using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class ProfileController(UserManager<AppUser> userManager) : Controller
    {


        public async Task<IActionResult> EditProfile()
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "User name not found");
                return View();
            }

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            var model = new UserEditViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };
            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if ((model.Password != null && model.PasswordConfirm != null) && (model.Password != model.PasswordConfirm))
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.UserName))
            {
                var Usr = await userManager.FindByNameAsync(model.UserName);

                if (Usr == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return View(model);
                }

                Usr.UserName = model.UserName;
                Usr.FirstName = model.FirstName ?? Usr.FirstName;
                Usr.LastName = model.LastName ?? Usr.LastName;
                Usr.City = model.City;
                Usr.PhoneNumber = model.PhoneNumber;
                Usr.ImageUrl = model.ImageUrl;
                Usr.Email = model.Email;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    Usr.PasswordHash = userManager.PasswordHasher.HashPassword(Usr, model.Password);
                }

                var updateResult = await userManager.UpdateAsync(Usr);
                if (updateResult.Succeeded)
                {
                    return View();
                }
                else
                {
                    foreach (var item in updateResult.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
                return View(model);
        }



    }
}
