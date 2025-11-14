using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models;
using NotikaIdentityEmail.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmailContext>(options => options.UseSqlServer("Server=LPTNET2116;Initial Catalog=NotikaEmailDb;Integrated Security=True;TrustServerCertificate=True;"));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EmailContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<EmailContext>().AddErrorDescriber<CustomIdentityValidator>(); // AddEntityFrameworkStores -> Her şeyi db'de tutacağım, hangi veritabanında tutayım? 
builder.Services.AddScoped<IEmailService,EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Register}/{action=CreateUser}/{id?}")
    .WithStaticAssets();


app.Run();
