namespace Sudoku.MVC.Exceptions;

public class UserNullException : Exception
{
    public UserNullException(string? message) : base(message)
    {
    }
}
