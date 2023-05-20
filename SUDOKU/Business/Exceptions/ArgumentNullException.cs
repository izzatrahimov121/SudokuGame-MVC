namespace Business.Exceptions;

public class ArgumentNullException : Exception
{
    public ArgumentNullException(string? message) : base(message)
    {
    }
}
