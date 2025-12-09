using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModels;
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

            var unreadCount = await context.Messages.CountAsync(x => x.ReceiverEmail == user.Email && !x.IsRead);

            List<MessageWithUserPhotosViewModel> model = await (
                from m in context.Messages
                join u in context.Users on m.SenderEmail equals u.Email
                where m.ReceiverEmail == user.Email && m.IsRead == false
                select new MessageWithUserPhotosViewModel
                {
                    FullName = u.FirstName + " " + u.LastName,
                    Message = m,
                    ImageUrl = u.ImageUrl
                }
            ).ToListAsync();
            return View(model);
        }


    }
}
