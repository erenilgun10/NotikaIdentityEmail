using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Services;
using System;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, EmailContext context) : Controller
    {

        public async Task<IActionResult> RoleList()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }


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

        public async Task<IActionResult> DeleteRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest();
            }

            var role = await roleManager.FindByIdAsync(roleId);
            if (role != null && role.Id != Guid.Empty.ToString())
            {
                await roleManager.DeleteAsync(role);
            }

            return RedirectToAction("RoleList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string Id)
        {
            UpdateRoleViewModel model = new();

            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var role = await roleManager.FindByIdAsync(Id);
            if (role != null)
            {

                model.RoleId = role.Id;
                model.RoleName = role.Name ?? string.Empty;

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleViewModel model)
        {

            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role != null)
            {

                role.Id = model.RoleId;
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var userList = await userManager.Users.ToListAsync();

            return View(userList);
        }

        [HttpGet]
        public async Task<IActionResult> AssingRole(string Id)
        {
            var roles = roleManager.Roles.ToList();
            ViewBag.v = roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            var user = await userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var model = new CreateUserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                return View(model);

            }
            else
            {
                return RedirectToAction("UserList");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AssingRole(CreateUserRoleViewModel model)
        {
            
            if (model.UserId == null)
            {
                ModelState.AddModelError("", "UserId is required");
                return View();
            }

            if (string.IsNullOrEmpty(model.RoleId))
            {
                ModelState.AddModelError("", "RoleId is required");
                return View();
            }
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role != null)
            {
                var result = await userManager.AddToRoleAsync(user, role.Name ?? string.Empty);
                if (result.Succeeded)
                {
                    return RedirectToAction("AssingRole", "Role", new { id = user.Id });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Role not found");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RemoveRole(string UserId,string RoleId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı");
                return View();
            }

            if (string.IsNullOrEmpty(RoleId))
            {
                ModelState.AddModelError("", "Rol Bulunamadı");
                return View();
            }
            var user = await userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            var role = await roleManager.FindByIdAsync(RoleId);
            if (role != null)
            {
                var result = await userManager.RemoveFromRoleAsync(user, role.Name ?? string.Empty);
                if (result.Succeeded)
                {
                    return RedirectToAction("AssingRole", "Role", new { id = user.Id });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Role not found");
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetUserRole(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Json(new { success = false, message = "İlgili kullanıcı bulunamamıştır." });

            var userRoles = await (from ur in context.UserRoles
                                   join r in context.Roles on ur.RoleId equals r.Id
                                   where ur.UserId == Id
                                   select new
                                   {
                                      ur.RoleId,
                                      r.Name,
                                   }).ToListAsync();
            if (userRoles == null || userRoles.Count == 0)
                return Json(new { success = false, message = "Bu kullanıcıya atanmış rol bulunamadı" });
            else
                return Json(new { success = true, data = userRoles });

        }












    }
}
