namespace Aviant.DDD.Application.Exceptions
{
    using System;
    using Domain.Exceptions;

    public class ApplicationException : ExceptionBase
    {
        public ApplicationException(string errorMessage)
            : base(errorMessage)
        {
        }

        public ApplicationException(string name, object key)
            : base(name, key)
        {
        }

        public ApplicationException(string errorMessage, Exception exception)
            : base(errorMessage, exception)
        {
        }
    }
}