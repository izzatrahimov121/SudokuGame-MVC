namespace Sudoku.MVC.Exceptions;

public class TimeIsOverException : Exception
{
	public TimeIsOverException(string? message) : base(message)
	{
	}
}
