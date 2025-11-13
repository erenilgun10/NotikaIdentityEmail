using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents;

public class MessageCategoryListSideBar(EmailContext context) : ViewComponent
{
    private readonly EmailContext _context = context;

    public IViewComponentResult Invoke()
    {
        var values = _context.Categories.ToList();
        return View(values);
    }
}
