namespace Aviant.DDD.Domain
{
    public class Exception : System.Exception
    {
        public Exception(string errorMessage) : base(errorMessage, null) {}
        
        public Exception(string errorMessage, System.Exception exception) :
            base($"The following error occurred \"{errorMessage}\"", exception) {}
    }
}