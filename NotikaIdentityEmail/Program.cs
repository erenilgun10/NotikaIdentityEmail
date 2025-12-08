using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmailContext>(options =>
    options.UseSqlServer("Server=LPTNET2116;Initial Catalog=NotikaEmailDb;Integrated Security=True;TrustServerCertificate=True;"));

builder.Services.AddControllersWithViews();

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<EmailContext>()
    .AddErrorDescriber<CustomIdentityValidator>();

builder.Services.AddScoped<IEmailService, EmailService>();

#region JWT Settings

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettingsModel>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettingsModel>();

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("Jwt ayarları (özellikle SecretKey) düzgün yapılandırılmamış. appsettings.json dosyasını kontrol edin.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=UserLogin}/{id?}")
    .WithStaticAssets();

app.Run();
