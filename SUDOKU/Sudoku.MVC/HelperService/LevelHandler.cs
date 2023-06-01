using Core.Enums;

namespace Sudoku.MVC.HelperService;

public static class LevelHandler
{
	public static string GetLevel(int totalScore)
	{
		if (totalScore >= 100 && totalScore < 200) return Levels.Bronze2.ToString(); 
		else if (totalScore >= 200 && totalScore < 350) return Levels.Bronze3.ToString(); 
		else if (totalScore >= 350 && totalScore < 500) return Levels.Silver1.ToString(); 
		else if (totalScore >= 500 && totalScore < 700) return Levels.Silver2.ToString(); 
		else if (totalScore >= 700 && totalScore < 950) return Levels.Silver3.ToString(); 
		else if (totalScore >= 950 && totalScore < 1150) return Levels.Platinum1.ToString(); 
		else if (totalScore >= 1150 && totalScore < 1350) return Levels.Platinum2.ToString(); 
		else if (totalScore >= 1350 && totalScore < 1500) return Levels.Platinum3.ToString(); 
		else if (totalScore >= 1500 && totalScore < 1650) return Levels.Diamond1.ToString(); 
		else if (totalScore >= 1650 && totalScore < 1800) return Levels.Diamond2.ToString(); 
		else if (totalScore >= 1800 && totalScore < 2000) return Levels.Diamond3.ToString(); 
		else if (totalScore >= 1800 && totalScore < 2000) return Levels.Diamond3.ToString(); 
		else if (totalScore >= 2000) return Levels.Master.ToString(); 

		return Levels.Bronze1.ToString();
	}
}
