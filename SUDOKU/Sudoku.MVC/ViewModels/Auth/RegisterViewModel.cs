using System.ComponentModel.DataAnnotations;

namespace Sudoku.MVC.ViewModels.Auth;

public class RegisterViewModel
{
    [Required,DataType(DataType.EmailAddress),MaxLength(255)]
    public string? Email { get; set; }

    [Required, MaxLength(150)]
    public string? UserName { get; set; }

    [Required, DataType(DataType.Password), MinLength(8)]
    public string? Password { get; set; }

    [Required, DataType(DataType.Password), Compare("Password")]
    public string? ConfirmPassword { get; set; }
}
