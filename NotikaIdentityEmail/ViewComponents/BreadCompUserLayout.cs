using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class BreadCompUserLayout : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
