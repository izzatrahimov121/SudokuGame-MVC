using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class ResetPasswordViewModel
{
	[Required(ErrorMessage = "Boş buraxmayın"), DataType(DataType.Password), MinLength(8, ErrorMessage = "Minimum 8 sinvol")]
	public string NewPassword { get; set; }

	[Required(ErrorMessage = "Boş buraxmayın"), DataType(DataType.Password), Compare(nameof(NewPassword))]
	public string ConfirmPassword { get; set; }

	public string email { get; set; }
	public string token { get; set; }

}
