using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class HeaderUserLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
