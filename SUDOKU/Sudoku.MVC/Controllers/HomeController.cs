using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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





    public async Task<ActionResult> Index()
    {
        HomeViewModel model = new();
        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is null)
        {
            return View(model);
        }
        var user = await _userManager.FindByNameAsync(userName: loginUser);
        if (user is not null)
        {
            model.UserName = user.UserName;
            model.UserProfilPhoto = user.ProfilPhoto;
            model.Level = user.Level;
            model.WorldRanking = (await _worldRaytingRepository.FindAll().Where(x => x.UserId == user.Id).ToListAsync())[0].Rayting;
        }
        return View(model);
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
        ViewBag.Mode = GameMode;
        HomeViewModel model = new();

        var loginUser = HttpContext.User.Identity?.Name;
        if (loginUser is not null)
        {
            var user = await _userManager.FindByNameAsync(userName: loginUser);
            model.UserName = user.UserName;
            model.UserProfilPhoto = user.ProfilPhoto;
            model.Level = user.Level;
            model.WorldRanking = (await _worldRaytingRepository.FindAll().Where(x => x.UserId == user.Id).ToListAsync())[0].Rayting;
        }
        model.Matrix = CreatedSudoku.Create(GameMode);
        model.FullMatrix = CreatedSudoku.fullsudoku();
        return View(model);
    }




    [HttpPost]
    [Route("/home/gamepost")]
    public async Task<IActionResult> GamePost([FromServices] IUrlHelperFactory urlHelperFactory)
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
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }

        var urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
        string redirectUrl = urlHelper.Action("Index", "Home");
        return Json(new { redirectUrl });
    }




    private static int nextcount { get; set; } = 0;
    public async Task<IActionResult> WorldRayting(string type = "null")
    {
        HomeViewModel model = new();
        if (type == "next")
        {
            nextcount++;
            model.Raytings = await _worldRaytingRepository.FindAll().OrderBy(x => x.Rayting).Skip(nextcount * 5).Take(5).ToListAsync();
        }
        else if (type == "previous")
        {
            if (nextcount == 1 || nextcount == 0)
            {
                model.Raytings = await _worldRaytingRepository.FindAll().OrderBy(x => x.Rayting).Take(5).ToListAsync();
                nextcount = 0;
            }
            else if (nextcount > 1)
            {
                model.Raytings = await _worldRaytingRepository.FindAll().OrderBy(x => x.Rayting).Skip((nextcount - 1) * 5).Take(5).ToListAsync();
                nextcount--;
            }
        }
        else
        {
            nextcount = 0;
            model.Raytings = await _worldRaytingRepository.FindAll().OrderBy(x => x.Rayting).Take(5).ToListAsync();
        }

        return View(model);
    }



    [HttpPost]
    public IActionResult POSTWorldRayting([FromServices] IUrlHelperFactory urlHelperFactory)
    {
        string type = Request.Form["btn1"];
        var urlHelper = urlHelperFactory.GetUrlHelper(ControllerContext);
        string redirectUrl = urlHelper.Action("WorldRayting", "Home", new { type });
        return Json(new { redirectUrl });
    }

}
