namespace Sudoku.MVC.HelperService.Interfaces;

public interface IFileService
{
	Task<string> CopyFileAsync(IFormFile file, string wwwroot, params string[] folders);
}
