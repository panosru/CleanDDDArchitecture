namespace Aviant.DDD.Domain.Exceptions
{
    using System;

    public abstract class ExceptionBase : Exception //TODO: Revisit exceptions (here and in Application also)
    {
        private ExceptionBase()
        {
        }

        public ExceptionBase(string errorMessage)
            : base(errorMessage, null)
        {
        }

        public ExceptionBase(string name, object key)
        {
        }

        public ExceptionBase(string errorMessage, Exception exception)
            :
            base($"The following error occurred \"{errorMessage}\"", exception)
        {
        }
    }
}