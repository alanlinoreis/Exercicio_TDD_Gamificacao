using System;
namespace Gamification.Domain.Exceptions;
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
public class IneligibleException : DomainException
{
    public IneligibleException(string message) : base(message) { }
}
public class AlreadyGrantedException : DomainException
{
    public AlreadyGrantedException(string message) : base(message) { }
}
public class ConfigurationException : DomainException
{
    public ConfigurationException(string message) : base(message) { }
}
