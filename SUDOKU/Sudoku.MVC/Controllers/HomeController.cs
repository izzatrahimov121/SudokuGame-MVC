using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sudoku.MVC.HelperService;
using Sudoku.MVC.ViewModels.Home;

namespace Sudoku.MVC.Controllers;

public class HomeController : Controller
{
	private readonly UserManager<AppUser> _userManager;

	public HomeController(UserManager<AppUser> userManager)
	{
		_userManager = userManager;
	}

	public static string? GameMode { get; set; }


	public async Task<IActionResult> Index()
	{

		var loginUser = HttpContext.User.Identity?.Name;
		if (loginUser is null)
		{
			ViewBag.UserName = "Player";
			ViewBag.UserProfilPhoto = "default_user_photo.png";
			ViewBag.Level = Levels.Bronze1.ToString();
			ViewBag.WorldRanking = "0";
			return View();
		}
		var user = await _userManager.FindByNameAsync(userName: loginUser);
		ViewBag.UserName = user.UserName;
		ViewBag.UserProfilPhoto = user.ProfilPhoto;
		ViewBag.Level = user.Level;
		ViewBag.WorldRanking = "00";
		return View();
	}


	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Index(HomeViewModel model)
	{
		GameMode = model.GameMode;
		return RedirectToAction(nameof(Game));
	}


	public IActionResult Game()
	{
		ViewBag.Mode = GameMode;
		GameViewModel gameVM = new();
		gameVM.Matrix = CreatedSudoku.Create(GameMode);
		gameVM.FullMatrix = CreatedSudoku.fullsudoku();
		return View(gameVM);
	}
}
