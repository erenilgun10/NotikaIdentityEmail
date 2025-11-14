
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.LoginViewComponents;

public class LoginHead : ViewComponent
{
    public IViewComponentResult Invoke(string title)
    {
        
        return View("Default",title);

    }

}
