using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Enum;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Services;
using System;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(EmailContext context, UserManager<AppUser> userManager) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await userManager.Users.ToListAsync();
            return View(users);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user is not null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

            }
            return RedirectToAction("UserList");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateUserIsActive(string id, bool isActive)
        {
            var user = await userManager.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (user is null)
                return RedirectToAction("UserCommentList");

            user.IsActive = isActive;
            await context.SaveChangesAsync();

            return RedirectToAction("UserList");
        }

    }
}
