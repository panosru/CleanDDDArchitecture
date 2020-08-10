namespace Aviant.DDD.Application.Exception
{
    using System;
    using Domain.Exceptions;

    public class NotFoundException : ExceptionBase
    {
        public NotFoundException(string message) :
            base(message)
        {
        }

        public NotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
        
        public NotFoundException(string name, object key) : 
            base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}