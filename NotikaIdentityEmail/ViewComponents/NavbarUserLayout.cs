using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class NavbarUserLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
