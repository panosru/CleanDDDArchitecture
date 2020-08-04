namespace Aviant.DDD.Application.Exception
{
    using System;
    using Domain.Exception;
    
    public class NotFound : Base
    {
        public NotFound(string name, object key) : 
            base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}