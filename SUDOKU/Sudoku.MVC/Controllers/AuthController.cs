using Core.Entities;
using Core.Enums;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sudoku.MVC.Exceptions;
using Sudoku.MVC.HelperService;
using Sudoku.MVC.HelperService.Interfaces;
using Sudoku.MVC.ViewModels.Auth;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Sudoku.MVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _context;
    private readonly IWorldRaytingRepository _worldRaytingRepository;
    private readonly IFileService _fileService;
    private readonly IHostingEnvironment _env;
    private readonly IAppUserRepository _appUserRepository;
    private readonly IMailService _mailService;
    public AuthController(UserManager<AppUser> userManager
        , SignInManager<AppUser> signInManager
        , RoleManager<IdentityRole> roleManager
        , AppDbContext context
        , IFileService fileService
        , IHostingEnvironment env
        , IAppUserRepository appUserRepository
        , IMailService mailService
        , IWorldRaytingRepository worldRaytingRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
        _fileService = fileService;
        _env = env;
        _appUserRepository = appUserRepository;
        _mailService = mailService;
        _worldRaytingRepository = worldRaytingRepository;
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
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(registerVM);
        }
        await _userManager.AddToRoleAsync(appUser, Roles.BasicUser.ToString());
        WorldRayting raytingDto = new()
        {
            Photo = appUser.ProfilPhoto,
            ThreeStar = 0,
            TotalScore = 0,
            UserName = appUser.UserName,
            UserId = appUser.Id,
            Rayting = 0,
        };
        await _worldRaytingRepository.CreateAsync(raytingDto);
        await _worldRaytingRepository.SaveAsync();

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

        user.IsActive = true;
        user.LastLoginDate = DateTime.Now;
        table.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    #endregion


    #region Change User Information

    public async Task<IActionResult> UserProfile()
    {
        UserProfileViewModel model = new();
        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is null)
        {
            model.ProfilPhoto = "default_user_photo.png";
            model.Username = "Player";
            model.Email = "exp@gmail.com";
            model.Phone = "";
            model.CompletedGames = 0;
            model.ThreeStar = 0;
            model.TotalScore = 0;
            return View(model);
        }
        var user = await _userManager.FindByNameAsync(userName: loginUser);
        model.ProfilPhoto = user.ProfilPhoto;
        model.Username = user.UserName;
        model.Email = user.Email;
        model.Phone = user.PhoneNumber;
        model.CompletedGames = user.CompletedGames;
        model.ThreeStar = user.SuccessfulGames;
        model.TotalScore = user.TotalScore;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserProfile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Məlumatların düzgün daxil edildiyindən əmin olun");
        }

        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
        if (user == null) return RedirectToAction(nameof(Login));

        var fileName = user.ProfilPhoto;
        if (model.Image != null)
        {
            fileName = await _fileService.CopyFileAsync(model.Image, _env.WebRootPath, "assets", "images", "user");
        }
        user.UserName = model.Username;
        user.Email = model.Email;
        user.ProfilPhoto = fileName;
        user.PhoneNumber = model.Phone;
        user.Email = model.Email;
        await _userManager.UpdateAsync(user);
        await _appUserRepository.SaveAsync();
        var Newuser = await _userManager.FindByEmailAsync(model.Email);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index","Home");
    }
    #endregion



    #region Reset Password

    private static int Code { get; set; }
    private static DateTime CodeLifeTime { get; set; }
    private async Task SendEmailCode(string email)
    {
        Random rnd = new Random();
        var code = rnd.Next(100000, 999999);
        MailRequestDto mailRequest = new()
        {
            ToEmail = email,
            Subject = "Account verification process",
            Body = $"<h3>Doğrulama kodunuz: {code} </h3>" +
            $"<br>Kodun etibarlılıq tarixi : {DateTime.Now.AddSeconds(300).ToString("dd MMMM yyyy HH: mm:ss")}<br>Kodu heçkimlə paylaşmayın <br> " +
            $"Copyright ©{DateTime.UtcNow.Year} Sudoku.az | All rights reserved."
        };
        await _mailService.SendEmailAsync(mailRequest);
        Code = code;
        CodeLifeTime = DateTime.Now.AddSeconds(300);
    }

    public IActionResult Verification()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> VerificationPost()
    {
        int code = int.Parse(Request.Form["code"]);
        if (DateTime.Now >= CodeLifeTime)
        {
            throw new TimeIsOverException("Time is over!");
        }
        if(code == Code)
        {
			return RedirectToAction("Index", "Home");
		}
		return RedirectToAction("Login","Auth");
    }
    #endregion











    ///////////Create User Roles///////////////////
    public async Task<IActionResult> CreateRoles()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                await _roleManager.CreateAsync(new() { Name = role.ToString() });
            }
        }
        return Content("ok");
    }

}
