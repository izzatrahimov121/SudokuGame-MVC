using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class UserProfileViewModel
{
	[Required, MaxLength(255)]
	public string? Username { get; set; } 


	public string? ProfilPhoto { get; set; }
	public IFormFile? Image { get; set; }


	[Required, DataType(DataType.EmailAddress), MaxLength(255)] 
	public string? Email { get; set; }


	[DataType(DataType.PhoneNumber), MaxLength(20)]
	public string? Phone { get; set; }


	[DataType(DataType.Password), MinLength(8)]
	public string? Password { get; set; }


	[DataType(DataType.Password), Compare(nameof(Password)), MinLength(8)]
	public string? ConfirmPassword { get; set; }


	public int CompletedGames { get; set; } = 0;
	public int ThreeStar { get; set; } = 0;
	public int TotalScore { get; set; } = 0;
}
