using Core.Enums;

namespace Sudoku.MVC.ViewModels.Home;

public class LayoutViewModel
{
    public string UserName { get; set; } = "Player";
    public string UserProfilPhoto { get; set; } = "default_user_photo.png";
    public string Level { get; set; } = Levels.Bronze1.ToString();
    public string WorldRanking { get; set; } = "0";
}
