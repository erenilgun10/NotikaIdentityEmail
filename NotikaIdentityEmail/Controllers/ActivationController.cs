using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Models;

namespace NotikaIdentityEmail.Controllers;


public class ActivationController(EmailContext context) : Controller
{
    private readonly EmailContext _context = context;


    [HttpGet]
    public IActionResult UserActivation()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UserActivation(int activateCode)
    {

        string? email = TempData["EmailKey"] as string;
        if (string.IsNullOrEmpty(email))
        {
            ModelState.AddModelError("Aktivasyon", "Aktivasyon kodu tek kullanımlıktır. Lütfen tekrar kayıt olmayı deneyin.");
            return RedirectToAction("CreateUser", "Register");
        }
        var code = _context.Users.Where(x => x.Email == email).Select(x => x.ActivationCode).FirstOrDefault();
        if (activateCode == code)
        {
            return RedirectToAction("UserLogin", "Login");
        }

        ModelState.AddModelError("", "Aktivasyon kodu hatalı.");
        return View();
    }
}


