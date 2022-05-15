namespace Patronum.Authricator;

public class AuthricatorException : Exception
{
    public new object? Data { get; }

    public AuthricatorException(string message, object? data=null) : base(message)
    {
        Data = data;
    }
}