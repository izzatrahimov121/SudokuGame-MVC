using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class LoginViewModel
{
    [Required, MaxLength(255)]
    public string? UserNameOrEmail { get; set; } = null!;



    [Required, DataType(DataType.Password), MinLength(8)]
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}
