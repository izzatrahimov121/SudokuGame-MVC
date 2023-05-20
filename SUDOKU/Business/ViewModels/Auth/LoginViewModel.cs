using System.ComponentModel.DataAnnotations;

namespace Business.ViewModels.Auth;

public class LoginViewModel
{
    [Required, DataType(DataType.EmailAddress), MaxLength(255)]
    public string? EmailAddress { get; set; } = null!;



    [Required, DataType(DataType.Password), MinLength(8)]
    public string? Password { get; set; } = null!;
}
