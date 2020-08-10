namespace Aviant.DDD.Application.Exceptions
{
    using System;

    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message)
            :
            base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }

        public NotFoundException(string name, object key)
            :
            base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}