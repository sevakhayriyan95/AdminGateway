namespace AdminGateway.Exceptions;

public class IncorrectDataException : ApplicationException
{
    public IncorrectDataException(string message) : base(message)
    {
    }
}