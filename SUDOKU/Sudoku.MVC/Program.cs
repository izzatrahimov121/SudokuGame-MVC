

using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Implementations;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sudoku.MVC.HelperService;
using Sudoku.MVC.HelperService.Implementations;
using Sudoku.MVC.HelperService.Interfaces;
using Sudoku.MVC.Utilites.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});



builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();//for frogot passwod



//add services
builder.Services.AddScoped<WorldRayting>();
builder.Services.AddScoped<IWorldRaytingRepository, WorldRaytingRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IMailService, MailService>();

//add BackgroundService
builder.Services.AddHostedService<MyBackgroundService>();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
//end BackgroundService





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
	endpoints.MapDefaultControllerRoute();
});
app.UseStaticFiles();



app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );


app.Run();
