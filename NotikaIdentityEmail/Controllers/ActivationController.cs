using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class ActivationController : Controller
    {
        [HttpGet]
        public IActionResult UserActivation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserActivation(string x)
        {
            return RedirectToAction("UserLogin", "Login");
        }



    }
}
