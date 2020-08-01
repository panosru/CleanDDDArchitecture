namespace Aviant.DDD.Domain.Exception
{
    public class Create : Base
    {
        public Create(string errorMessage) : base(errorMessage)
        {
        }

        public Create(string errorMessage, System.Exception exception) : 
            base(errorMessage, exception)
        {
        }
    }
}