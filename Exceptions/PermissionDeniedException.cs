namespace AdminGateway.Exceptions;

public class PermissionDeniedException : ApplicationException
{
    public PermissionDeniedException(string message) : base(message)
    {
    }
}