using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents
{
    public class TopBarUserLayout() : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }


    }
}
