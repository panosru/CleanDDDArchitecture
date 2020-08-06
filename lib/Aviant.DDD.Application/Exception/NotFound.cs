namespace Aviant.DDD.Application.Exception
{
    using System;
    using Domain.Exception;
    
    public class NotFound : Base
    {
        public NotFound(string message) :
            base(message)
        {
        }

        public NotFound(string message, Exception innerException) :
            base(message, innerException)
        {
        }
        
        public NotFound(string name, object key) : 
            base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}