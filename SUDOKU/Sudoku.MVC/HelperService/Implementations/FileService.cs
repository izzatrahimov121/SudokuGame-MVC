using Sudoku.MVC.Exceptions;
using Sudoku.MVC.HelperService.Interfaces;
using Sudoku.MVC.Utilites.Extensions;

namespace Sudoku.MVC.HelperService.Implementations;

public class FileService : IFileService
{
    public async Task<string> CopyFileAsync(IFormFile file, string wwwroot, params string[] folders)
    {
        string fileName = string.Empty;

        if (file is not null)
        {
            if (!file.CheckFileFormat("image/"))
            {
                throw new IncorrectFileFormatException("Incorrect file format");
            }
            if (!file.CheckFileSize(20))
            {
                throw new IncorrectFileFormatException("Incorrect file size");
            }
            fileName = Guid.NewGuid().ToString() + file.FileName;

            string resultPath = wwwroot;

            foreach (var folder in folders)
            {
                resultPath = Path.Combine(resultPath, folder);
            }
            resultPath = Path.Combine(resultPath, fileName);

            using (FileStream stream = new FileStream(resultPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        throw new Exception();
    }
}
