using Core.Enums;

namespace Sudoku.MVC.ViewModels.Home;

public class LayoutViewModel
{
    public string? UserProfilPhoto { get; set; } = "default_user_photo.png";
    public string? UserName { get; set; } = "Player";
    public string? Level { get; set; } = Levels.Bronze1.ToString();
    public int? WorldRanking { get; set; } = 0;
}
