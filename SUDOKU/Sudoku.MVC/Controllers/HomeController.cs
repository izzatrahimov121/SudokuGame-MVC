using Core.Entities;
using Core.Enums;
using DataAccess.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sudoku.MVC.HelperService;
using Sudoku.MVC.ViewModels.Home;

namespace Sudoku.MVC.Controllers;

public class HomeController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly AppDbContext _context;

	public HomeController(UserManager<AppUser> userManager, AppDbContext context)
	{
		_userManager = userManager;
		_context = context;
	}

	private DbSet<AppUser> _table => _context.Set<AppUser>();

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

	
	public async Task<IActionResult> Game()
	{

		//var loginUser = HttpContext.User.Identity?.Name;
		//if (loginUser is null)
		//{
		//	ViewBag.UserName = "Player";
		//	ViewBag.UserProfilPhoto = "default_user_photo.png";
		//	ViewBag.Level = Levels.Bronze1.ToString();
		//	ViewBag.WorldRanking = "0";
		//	return View();
		//}
		//var user = await _userManager.FindByNameAsync(userName: loginUser);
		//ViewBag.UserName = user.UserName;
		//ViewBag.UserProfilPhoto = user.ProfilPhoto;
		//ViewBag.Level = user.Level;
		//ViewBag.WorldRanking = "00";

		ViewBag.Mode = GameMode;
		GameViewModel gameVM = new();
		gameVM.Matrix = CreatedSudoku.Create(GameMode);
		gameVM.FullMatrix = CreatedSudoku.fullsudoku();
		return View(gameVM);
	}



	[HttpPost]
	[Route("/home/gamepost")]
	public async Task<IActionResult> GamePost()
	{
		int star = int.Parse(Request.Form["star"]);
		int score = int.Parse(Request.Form["score"]);

		var loginUser = HttpContext.User.Identity?.Name;
		if (loginUser is not null)
		{
			var user = await _userManager.FindByNameAsync(userName: loginUser);
			if (star == 3) { user.SuccessfulGames += 1; }
			user.TotalScore += score;
			if (user.TotalScore < 0) { user.TotalScore = 0; }
			user.CompletedGames+= 1;
			user.Level = LevelHandler.GetLevel(user.TotalScore);
			await _context.SaveChangesAsync();
		}

		return RedirectToAction(nameof(Index));
	}

	
}
