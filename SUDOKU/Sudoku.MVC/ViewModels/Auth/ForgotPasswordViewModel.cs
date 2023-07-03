using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class ForgotPasswordViewModel
{

	[Required(ErrorMessage ="Boş buraxmayın"),DataType(DataType.EmailAddress,ErrorMessage ="Email doğru deyil")]
	public string Email { get; set; }

	public string? ErrorMessage { get; set; }
}
