namespace Sudoku.MVC.ViewModels.Auth;

public class VerificationViewModel
{
    public string? Username { get; set; }
	public string? Image { get; set; }
	public string? Email { get; set; }
	public string? Phone { get; set; }
	public string? Password { get; set; }
	public string? ErrorMessage { get; set; }
}
