using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.TopBarViewComponents
{
    public class TopBarUserLayout() : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }


    }
}
