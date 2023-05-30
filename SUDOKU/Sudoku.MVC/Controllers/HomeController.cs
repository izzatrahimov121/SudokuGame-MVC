using Core.Entities;
using Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

	
	public async Task<IActionResult> Game()
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
		if (loginUser != null)
		{
			var user = await _userManager.FindByNameAsync(userName: loginUser);
			if (star == 3) { user.SuccessfulGames += 1; }
			user.TotalScore += score;
			if (user.TotalScore < 0) { user.TotalScore = 0; }
			user.CompletedGames+= 1;
			if (user.TotalScore >= 100 && user.TotalScore<200)  { user.Level= Levels.Bronze2.ToString(); }
			else if (user.TotalScore >= 200 && user.TotalScore<350)  { user.Level= Levels.Bronze3.ToString(); }
			else if(user.TotalScore >= 350 && user.TotalScore<500)  { user.Level= Levels.Silver1.ToString(); }
			else if (user.TotalScore >= 500 && user.TotalScore<700)  { user.Level= Levels.Silver2.ToString(); }
			else if (user.TotalScore >= 700 && user.TotalScore<950)  { user.Level= Levels.Silver3.ToString(); }
			else if (user.TotalScore >= 950 && user.TotalScore<1150) { user.Level= Levels.Platinum1.ToString(); }
			else if (user.TotalScore >= 1150 && user.TotalScore<1350) { user.Level= Levels.Platinum2.ToString(); }
			else if (user.TotalScore >= 1350 && user.TotalScore<1500) { user.Level= Levels.Platinum3.ToString(); }
			else if (user.TotalScore >= 1500 && user.TotalScore<1650) { user.Level= Levels.Diamond1.ToString(); }
			else if (user.TotalScore >= 1650 && user.TotalScore<1800) { user.Level= Levels.Diamond2.ToString(); }
			else if (user.TotalScore >= 1800 && user.TotalScore<2000) { user.Level= Levels.Diamond3.ToString(); }
			else if (user.TotalScore >= 1800 && user.TotalScore<2000) { user.Level= Levels.Diamond3.ToString(); }
			else if (user.TotalScore >= 2000) { user.Level= Levels.Master.ToString(); }
		}

		return RedirectToAction(nameof(Index));
	}

	
}
