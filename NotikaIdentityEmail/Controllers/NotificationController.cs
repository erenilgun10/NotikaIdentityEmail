using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Models.MessageViewModels;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize]
    public class NotificationController(EmailContext context) : Controller
    {
        public async Task<IActionResult> NotificationList()
        {


            var model = await context.Notifications.ToListAsync();

            return View(model);
        }
    }
}
