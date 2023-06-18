using Sudoku.MVC.ViewModels.Auth;

namespace Sudoku.MVC.HelperService.Interfaces;

public interface IMailService
{
	Task SendEmailAsync(MailRequestDto mailRequest);
}
