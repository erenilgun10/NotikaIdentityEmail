using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Inbox()
        {
            return View();
        }

    }
}
