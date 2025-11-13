using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class FooterUserLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
