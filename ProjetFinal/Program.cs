using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjetFinal.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Developpeurs/Login";
        o.LogoutPath = "/Developpeurs/Logout";
        o.AccessDeniedPath = "/Developpeurs/AccessDenied";
    });

builder.Services.AddAuthorization(o =>
    o.AddPolicy("CanEditProjects", p => p.RequireRole("Senior")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();   // <-- avant Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Developpeurs}/{action=Login}/{id?}"); // (option) démarrer sur Login

app.Run();
