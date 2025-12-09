using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.ForgetPasswordModels;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Services;

namespace NotikaIdentityEmail.Controllers
{
    public class PasswordChange(UserManager<AppUser> userManager, IEmailService emailService) : Controller
    {

        [HttpGet]
        public IActionResult ForgetPassword()
        {

            return View(new ForgetPasswordViewModel());
        }



        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            bool isSent = false;
            if (model.Email != null)
            {

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "PasswordChange", new { userId = user.Id, token }, Request.Scheme);
                    Console.WriteLine($"Şifre değiştirme Linki: {passwordResetLink}");
                    ViewBag.Message = "Şifre değiştirmek için gerekli link mail adresinize yollanmıştır.";

                    if (passwordResetLink != null)
                        isSent = await emailService.SendPasswordResetMailAsync(user, passwordResetLink);

                    if (isSent)
                        return RedirectToAction("UserLogin", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı.");
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }


        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordViewModel { UserId = userId, Token = token };

            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.UserId is not string userId || model.Token is not string token)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz istek.");
                return View(model);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password) || model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler birbirleriyle uyuşmamaktadır veya boş.");
                return View(model);
            }

            var result = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.Message = "Şifreniz başarıyla değiştirildi.";
                return RedirectToAction("UserLogin", "Login");
            }

            return View(model);
        }



    }


}