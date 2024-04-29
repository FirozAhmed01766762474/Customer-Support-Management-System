global using Microsoft.EntityFrameworkCore;
global using XFLCSMS.Models;
global using XFLCSMS.Data;
global using XFLCSMS.Services.EmailService;
using XFLCSMS.EmailService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(option =>
    {
    option.IdleTimeout = TimeSpan.FromMinutes(10);
    }
    ) ;

builder.Services.AddScoped<IEmailServices, EmailService>();
builder.Services.AddDbContext<DataContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=RegisterLogin}/{action=Login}/{id?}");
    pattern: "{controller=RegisterLogin}/{action=Login}/{id?}");

app.Run();
