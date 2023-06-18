using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class UserProfileViewModel
{
	[Required(ErrorMessage = "Boş buraxmayın"), MaxLength(255,ErrorMessage ="Usunluq 255-i keçib")]
	public string? Username { get; set; } 


	public string? ProfilPhoto { get; set; }
	public IFormFile? Image { get; set; }


	[Required(ErrorMessage = "Boş buraxmayın"), DataType(DataType.EmailAddress,ErrorMessage ="Düzgün mail dexil edin"), MaxLength(255)] 
	public string? Email { get; set; }


	[DataType(DataType.PhoneNumber,ErrorMessage ="Düzgün telefon nömrəsi daxil edin"), MaxLength(20,ErrorMessage ="Format düzgün deyil")]
	public string? Phone { get; set; }



	public int CompletedGames { get; set; } = 0;
	public int ThreeStar { get; set; } = 0;
	public int TotalScore { get; set; } = 0;
}
