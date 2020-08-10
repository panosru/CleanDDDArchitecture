namespace Aviant.DDD.Domain.Exceptions
{
    using System;

    public class DomainException : ExceptionBase
    {
        public DomainException(string errorMessage)
            : base(errorMessage)
        {
        }

        public DomainException(string errorMessage, Exception exception)
            :
            base($"The following error occurred \"{errorMessage}\"", exception)
        {
        }
    }
}