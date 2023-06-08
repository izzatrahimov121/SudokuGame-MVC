using Core.Entities;

namespace Sudoku.MVC.HelperService;

public class WorldRaytingDto
{
	public string UserId { get; set; }
	public string UserName { get; set; }
	public string Photo { get; set; }
	public int ThreeStar { get; set; }
	public int TotalScore { get; set; }
}
