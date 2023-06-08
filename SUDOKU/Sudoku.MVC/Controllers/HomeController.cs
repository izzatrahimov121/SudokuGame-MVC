using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;
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
    private readonly IWorldRaytingRepository _worldRaytingRepository;
    public HomeController(UserManager<AppUser> userManager, AppDbContext context, IWorldRaytingRepository worldRaytingRepository)
    {
        _userManager = userManager;
        _context = context;
        _worldRaytingRepository = worldRaytingRepository;
    }
    private DbSet<AppUser> _table => _context.Set<AppUser>();
    private static string? GameMode { get; set; }






    public async Task<IActionResult> Index(HomeViewModel model)
    {
        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is null)
        {
            return View(model);
        }
        var user = await _userManager.FindByNameAsync(userName: loginUser);


        //await _table1.AddAsync(rayting);

        model.UserName = user.UserName;
        model.UserProfilPhoto = user.ProfilPhoto;
        model.Level = user.Level;
        model.WorldRanking = "123";
        return View(model);
    }





    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index1(HomeViewModel model)
    {
        GameMode = model.GameMode;
        return RedirectToAction(nameof(Game));
    }




    public async Task<IActionResult> Game()
    {
        ViewBag.Mode = GameMode;
        HomeViewModel model = new();

        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is not null)
        {
            var user = await _userManager.FindByNameAsync(userName: loginUser);
            model.UserName = user.UserName;
            model.UserProfilPhoto = user.ProfilPhoto;
            model.Level = user.Level;
            model.WorldRanking = "123";
        }
        model.Matrix = CreatedSudoku.Create(GameMode);
        model.FullMatrix = CreatedSudoku.fullsudoku();
        return View(model);
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
            user.CompletedGames += 1;
            user.Level = LevelHandler.GetLevel(user.TotalScore);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> WorldRayting()
    {
        HomeViewModel model = new();
        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is not null)
        {
            var user = await _userManager.FindByNameAsync(userName: loginUser);
            model.UserName = user.UserName;
            model.UserProfilPhoto = user.ProfilPhoto;
            model.Level = user.Level;
            model.WorldRanking = "123";
        }

        model.Raytings= await _worldRaytingRepository.FindAll().OrderBy(x => x.Rayting).ToListAsync();
        return View(model);
    }

}
