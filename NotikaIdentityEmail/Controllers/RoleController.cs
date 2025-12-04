using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Services;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) : Controller
    {
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            IdentityRole identityRole = new()
            {
                Name = model.RoleName
            };
            await roleManager.CreateAsync(identityRole);

            return RedirectToAction("RoleList");
        }








    }
}
