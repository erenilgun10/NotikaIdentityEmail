using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Models.Jwt;
using NotikaIdentityEmail.Models.JwtModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers
{

    public class TokenController(IOptions<JwtSettingsModel> jwtSettings) : Controller
    {


        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Generate(SimpleUserViewModel simpleUserViewModel)
        {
            var claim = new[]
            {
                    new Claim("FirstName", simpleUserViewModel.FirstName),
                    new Claim("LastName", simpleUserViewModel.LastName),
                    new Claim("City", simpleUserViewModel.City),
                    new Claim("Username", simpleUserViewModel.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpireMinutes),
                signingCredentials: creds
            );
            simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token);

            return View(simpleUserViewModel);
        }

    }
}
