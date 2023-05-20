using Business.Services.Interfaces;
using Business.ViewModels.Auth;

namespace Business.Services.Implementations;

public class AuthService : IAuthService
{
    public Task LoginAsync(LoginViewModel loginVM)
    {
        throw new NotImplementedException();
    }

    public Task RegisterAsync(RegisterViewModel registerVM)
    {
        if ( is null)
        {
            throw new ArgumentNullException("null element");
        }
    }
}
