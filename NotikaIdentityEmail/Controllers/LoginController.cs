using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.Jwt;
using NotikaIdentityEmail.Models.JwtModels;
using NotikaIdentityEmail.Services;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController(SignInManager<AppUser> signInManager, EmailContext context, UserManager<AppUser> userManager, IOptions<JwtSettingsModel> jwt) : Controller
    {




        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usr = await context.Users.FirstOrDefaultAsync(u => u.UserName == model.Username);
            if (usr == null)
            {
                ModelState.AddModelError("", "Kullanıcı Adı ya da Şifre Yanlış");
                return View(model);
            }

            if (!usr.IsActive)
            {
                ModelState.AddModelError("", "Hesabınız devre dışı bırakılmıştır. Lütfen yöneticinizle iletişime geçiniz.");
                return View(model);
            }

            if (!usr.EmailConfirmed)
            {
                TempData["Error"] = "Kullanıcı hesabı henüz onaylı değildir. Lütfen onaylayınız.";
                return RedirectToAction("UserActivation", "Activation");
            }

            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabınız geçici olarak kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Kullanıcı Adı ya da Şifre Yanlış");
                return View(model);
            }

            var token = GenerateJwtToken(new SimpleUserViewModel
            {
                Id = usr.Id,
                FirstName = usr.FirstName,
                LastName = usr.LastName,
                City = usr.City ?? string.Empty,
                Email = usr.Email ?? string.Empty,
                Username = usr.UserName ?? string.Empty
            });

            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(jwt.Value.ExpireMinutes)
            });
            return RedirectToAction("Inbox", "Message");
        }

        [HttpPost]
        private string GenerateJwtToken(SimpleUserViewModel simpleUserViewModel)
        {
            var claim = new[]
            {
                new Claim("FirstName", simpleUserViewModel.FirstName),
                new Claim("LastName", simpleUserViewModel.LastName),
                new Claim("City", simpleUserViewModel.City),
                new Claim("Username", simpleUserViewModel.Username),
                new Claim(ClaimTypes.NameIdentifier, simpleUserViewModel.Id),
                new Claim(ClaimTypes.Email, simpleUserViewModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt.Value.Issuer,
                audience: jwt.Value.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(jwt.Value.ExpireMinutes),
                signingCredentials: creds
            );
            simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);

        }

        public async Task<IActionResult> ExternalLoginCallBack(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError(string.Empty, $"Dış Servis Hatası: {remoteError}");
                return RedirectToAction("UserLogin");
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("UserLogin");

            var signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
                return RedirectToAction("Inbox", "Message");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Dış servis email bilgisi döndürmedi.");
                return RedirectToAction("UserLogin");
            }

            var given = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var sur = info.Principal.FindFirstValue(ClaimTypes.Surname);

            string first = (given ?? "").Trim();
            string last = (sur ?? "").Trim();

            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
            {
                var fullName = (info.Principal.FindFirstValue(ClaimTypes.Name) ?? "").Trim();
                var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 1)
                {
                    first = string.IsNullOrWhiteSpace(first) ? parts[0] : first;
                    last = string.IsNullOrWhiteSpace(last) ? "" : last;
                }
                else if (parts.Length > 1)
                {
                    first = string.IsNullOrWhiteSpace(first) ? string.Join(' ', parts.Take(parts.Length - 1)) : first;
                    last = string.IsNullOrWhiteSpace(last) ? parts[^1] : last;
                }
            }

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                var userLogins = await userManager.GetLoginsAsync(existingUser);
                var alreadyLinked = userLogins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey);

                if (!alreadyLinked)
                {
                    var linkResult = await userManager.AddLoginAsync(existingUser, info);
                    if (!linkResult.Succeeded)
                        return RedirectToAction("UserLogin");
                }

                await signInManager.SignInAsync(existingUser, isPersistent: false);
                return RedirectToAction("Inbox", "Message");
            }

            var rawBase = $"{first}.{last}".Trim('.');
            var baseUserName = ToAsciiUserName(rawBase);

            if (string.IsNullOrWhiteSpace(baseUserName))
                baseUserName = ToAsciiUserName(email.Split('@')[0]);

            var rnd = Random.Shared;
            var suffix = rnd.Next(0, 2) == 0 ? "" : rnd.Next(10, 100).ToString();



            var user = new AppUser
            {
                Email = email,
                UserName = baseUserName + suffix,
                FirstName = first,
                LastName = last
            };

            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return RedirectToAction("Login");

            var addLoginResult = await userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
                return RedirectToAction("UserLogin");

            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Inbox", "Message");
        }

        static string ToAsciiUserName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var s = input.Trim().ToLowerInvariant();

            s = s
                .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
                .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");

            s = s.Replace(" ", "");

            s = Regex.Replace(s, @"[^a-z0-9._-]", "");

            s = s.Trim('.');

            return s;
        }




    }
}
