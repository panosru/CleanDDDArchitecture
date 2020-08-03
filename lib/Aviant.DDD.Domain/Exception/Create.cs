namespace Aviant.DDD.Domain.Exception
{
    using System;

    public class Create : Base
    {
        public Create(string errorMessage) : base(errorMessage)
        {
        }

        public Create(string errorMessage, Exception exception) :
            base(errorMessage, exception)
        {
        }
    }
}