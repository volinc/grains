namespace Grains.Shared.Contracts;

[GenerateSerializer]
[Alias("Grains.Interfaces.DomainException")]
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}