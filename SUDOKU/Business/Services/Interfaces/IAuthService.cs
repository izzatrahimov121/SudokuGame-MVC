using Business.ViewModels.Auth;

namespace Business.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterViewModel registerVM);
    Task LoginAsync(LoginViewModel loginVM);


}
