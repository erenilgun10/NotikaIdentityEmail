using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class MessageSideBar(EmailContext context, UserManager<AppUser> userManager) : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new MessageCountsViewModel();
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return View(0);

            var user = await userManager.FindByNameAsync(userName);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return View(0);

            model.InboxCount = await context.Messages.CountAsync(x => x.ReceiverEmail == user.Email);
            model.SentBoxCount = await context.Messages.CountAsync(x => x.SenderEmail == user.Email);
            return View(model);
        }
    }
}
