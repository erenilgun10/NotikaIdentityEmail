using Microsoft.AspNetCore.Mvc;
namespace NotikaIdentityEmail.ViewComponents
{
    public class MessageSideBar : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
