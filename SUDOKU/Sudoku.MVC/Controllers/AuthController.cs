using Core.Entities;
using Core.Enums;
using DataAccess.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sudoku.MVC.ViewModels.Auth;

namespace Sudoku.MVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;
    public AuthController(UserManager<AppUser> userManager
        , SignInManager<AppUser> signInManager
        , RoleManager<IdentityRole> roleManager
        ,AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }

    DbSet<AppUser> table => _context.Set<AppUser>();

    #region Register
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerVM)
    {
        if (!ModelState.IsValid) { return View(registerVM); }

        Levels level = (Levels)1;
        AppUser appUser = new()
        {
            UserName = registerVM.UserName,
            Email = registerVM.Email,
            IsActive = true,
            LastLoginDate = DateTime.Now,
            TotalScore = 0,
            CompletedGames = 0,
            SuccessfulGames = 0,
            Level = level.ToString()
        };

        var identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);
        if (!identityResult.Succeeded) 
        {
            foreach(var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(registerVM);
        }

        await _userManager.AddToRoleAsync(appUser,Roles.BasicUser.ToString());
        return RedirectToAction(nameof(Login));
    }
    #endregion

    #region Login
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginVM)
    {
        //if (!ModelState.IsValid) 
        //{
        //    ModelState.AddModelError("", "ModelState error");
        //    return View(loginVM); 
        //}
        var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);
        if (user is null)
        {
            user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Username/Email or Password incorrect");
                return View(loginVM);
            }
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, (bool)loginVM.RememberMe, true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError("", "Please try again soon");
            return View(loginVM);
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("", "Username/Email or Password incorrect");
            return View(loginVM);
        }

        user.IsActive=true;
        user.LastLoginDate= DateTime.Now;
        table.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    #endregion


    ///////////Create User Roles///////////////////
    public async Task<IActionResult> CreateRoles()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                await _roleManager.CreateAsync(new() { Name = role.ToString() }) ;
            }
        }

        return Content("ok");
    }

}
