using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModels;
namespace NotikaIdentityEmail.ViewComponents.TopBarViewComponents
{
    public class TopBarNotificationList(EmailContext context) : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notificationCount = await context.Notifications.CountAsync();

            List<Notification> model = await context.Notifications.OrderByDescending(n => n.NotificationId).Take(5).ToListAsync();
            ViewBag.NotificationCount = notificationCount;
            return View(model);
        }


    }
}
