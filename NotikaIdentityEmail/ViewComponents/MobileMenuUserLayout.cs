using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class MobileMenuUserLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
