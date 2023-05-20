namespace Business.ViewModels.Auth;

public class TokenResponseDto
{
    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public string? Username { get; set; }
}
