namespace Aviant.DDD.Domain.Exception
{
    using System;

    public abstract class Base : Exception
    {
        private Base()
        {}
        
        public Base(string errorMessage) : base(errorMessage, null)
        {
        }
        
        public Base(string name, object key)
        {}

        public Base(string errorMessage, Exception exception) :
            base($"The following error occurred \"{errorMessage}\"", exception)
        {
        }
    }
}