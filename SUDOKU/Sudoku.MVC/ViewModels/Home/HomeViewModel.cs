using Core.Entities;
using Core.Enums;
using System.Collections.Generic;

namespace Sudoku.MVC.ViewModels.Home;

public class HomeViewModel
{
    public string? GameMode { get; set; } = "default";


    public string UserName { get; set; } = "Player";
    public string UserProfilPhoto { get; set; } = "default_user_photo.png";
    public string Level { get; set; } = Levels.Bronze1.ToString();
    public int WorldRanking { get; set; } = 0;



    public int[,]? Matrix { get; set; }
    public int[,]? FullMatrix { get; set; }

    public List<WorldRayting> Raytings { get; set; }
}
