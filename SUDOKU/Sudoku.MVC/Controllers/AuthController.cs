using Core.Entities;
using Core.Enums;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Protocol.Plugins;
using Sudoku.MVC.Exceptions;
using Sudoku.MVC.HelperService;
using Sudoku.MVC.HelperService.Interfaces;
using Sudoku.MVC.ViewModels.Auth;
using System.Text;
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
		return RedirectToAction("Index", "Home");
	}
	#endregion



	#region Reset Password

	private async Task SendEmailUrl(string email, string url)
	{
		MailRequestDto mailRequest = new()
		{
			ToEmail = email,
			Subject = "Reset password",
			Body = $"<h3>Şifrəni dəyişmək üşün aşağıdakı kilikləyin</h3>" +
			$"<br><a href='{url}'>" +
			$"<button style=\"background-color: #9de2ff; width:100px;height:40px;\">Yeni şifrə təyin et</button>" +
			$"</a>" +
			$"<br>Linkin etibarlılıq tarixi : {DateTime.Now.AddSeconds(300).ToString("dd MMMM yyyy HH: mm:ss")} <br> " +
			$"Copyright ©{DateTime.UtcNow.Year} Sudoku.az | All rights reserved."
		};
		await _mailService.SendEmailAsync(mailRequest);
	}

	public IActionResult ForgotPassword(string? errorMessage)
	{
		ForgotPasswordViewModel model = new();
		model.ErrorMessage = errorMessage;
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
	{
		if (!ModelState.IsValid) { return View(model); }

		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user is null)
		{
			return RedirectToAction("ForgotPassword", new { errorMessage = "Bu emaillə istifadəçi tapılmadı" });
		}
		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
		//var callbackUrl = $"http://localhost:5256/api/Accounts/resetpassword?email={model.Email}&token={System.Web.HttpUtility.UrlEncode(token)}";
		var callbackUrl = Url.Action("ResetPassword", "Auth", new { email = model.Email, token = encodedToken }, protocol: HttpContext.Request.Scheme);
		await SendEmailUrl(model.Email, callbackUrl);
		return View("SuccessRefleshLink");
	}

	
	public IActionResult ResetPassword(string email, string token)
	{
		ResetPasswordViewModel model = new();
		model.email = email;
		model.token = token;
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
	{
		var user = await _userManager.FindByEmailAsync(model.email);
		var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.token));
		var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
		if (!result.Succeeded)
		{
			var errors = result.Errors.Select(e => e.Description).ToList();
			foreach (var error in errors)
			{
				ModelState.AddModelError("", error);
			}
			return View(model);
		}
		return RedirectToAction(nameof(Login));
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
