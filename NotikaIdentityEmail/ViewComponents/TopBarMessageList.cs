using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
namespace NotikaIdentityEmail.ViewComponents
{
    public class TopBarMessageList(EmailContext context, UserManager<AppUser> userManager) : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return View(0);

            var user = await userManager.FindByNameAsync(userName);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return View(0);

            var messages = await context.Messages.Where(x => x.ReceiverEmail == user.Email &&  x.IsRead == false).ToListAsync();
            return View(messages);
        }

        
    }
}
