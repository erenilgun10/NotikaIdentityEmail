using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.Jwt;
using NotikaIdentityEmail.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmailContext>(options =>
    options.UseSqlServer("Server=MachineName;Initial Catalog=DbName;Integrated Security=True;TrustServerCertificate=True;"));

builder.Services.AddControllersWithViews();

builder.Services
    .AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<EmailContext>()
    .AddErrorDescriber<CustomIdentityValidator>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

builder.Services.AddScoped<IEmailService, EmailService>();

#region JWT Settings

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettingsModel>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettingsModel>();

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
    throw new InvalidOperationException("Jwt ayarları düzgün yapılandırılmamış.");

#endregion

var clientId = builder.Configuration["GoogleConsoleCloud:ClientId"];
var secretKey = builder.Configuration["GoogleConsoleCloud:SecretKey"];

if (string.IsNullOrWhiteSpace(clientId))
    throw new InvalidOperationException("Google Console Cloud ClientId is not configured.");

if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("Google Console Cloud SecretKey is not configured.");

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var auth = context.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer "))
                {
                    context.Token = auth["Bearer ".Length..].Trim();
                    return Task.CompletedTask;
                }

                var token = context.Request.Cookies["jwtToken"];
                if (!string.IsNullOrWhiteSpace(token))
                    context.Token = token;

                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = clientId!;
        options.ClientSecret = secretKey!;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Error/{0}");

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
