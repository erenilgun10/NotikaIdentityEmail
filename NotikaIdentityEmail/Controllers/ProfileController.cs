using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers
{
    public class ProfileController : Controller
    {

        public IActionResult EditProfile()
        {
            return View();
        }




    }
}
