namespace Aviant.DDD.Domain.Exception
{
    public abstract class Base : System.Exception
    {
        public Base(string errorMessage) : base(errorMessage, null) {}
        
        public Base(string errorMessage, System.Exception exception) :
            base($"The following error occurred \"{errorMessage}\"", exception) {}
    }
}