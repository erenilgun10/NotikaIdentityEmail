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
                UserName = user.UserName ?? "NULL",
                FirstName = user.FirstName,
                LastName = user.LastName,
                City = user.City ?? "NULL",
                PhoneNumber = user.PhoneNumber ?? "NULL",
                ImageUrl = user.ImageUrl ?? "NULL",
            };
            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            var Usr = await userManager.FindByNameAsync(model.UserName);
            if (Usr == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            Usr.FirstName = model.FirstName != null && model.FirstName != Usr.FirstName ? model.FirstName : Usr.FirstName;
            Usr.LastName = model.LastName != null && model.LastName != Usr.LastName ? model.LastName : Usr.LastName;
            Usr.City = model.City != null && model.City != Usr.City ? model.City : Usr.City;
            Usr.PhoneNumber = model.PhoneNumber != null && model.PhoneNumber != Usr.PhoneNumber ? model.PhoneNumber : Usr.PhoneNumber;
            Usr.ImageUrl = model.ImageUrl != null && model.ImageUrl != Usr.ImageUrl ? model.ImageUrl : Usr.ImageUrl;
            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(Usr);
                var result = await userManager.ResetPasswordAsync(Usr, token, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View(model);
                }
            }
            var updateResult = await userManager.UpdateAsync(Usr);
            if (updateResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var item in updateResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(model);
            }

        }




    }
}
