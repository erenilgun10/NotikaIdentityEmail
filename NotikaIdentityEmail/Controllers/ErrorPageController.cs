using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class ErrorPageController : Controller
    {
        [Route("Error/404")]
        public IActionResult Page404()
        {
            return View();
        }
        [Route("Error/401")]
        public IActionResult Page401()
        {
            return View();
        }
        [Route("Error/403")]
        public IActionResult Page403()
        {
            return View();
        }

        [Route("Error/{errorCode}")]
        public IActionResult HandleError(int errorCode)
        {
            return errorCode switch
            {
                404 => RedirectToAction("Page404"),
                401 => RedirectToAction("Page401"),
                403 => RedirectToAction("Page403"),
                _ => View(),
            };
        }

    }
}
