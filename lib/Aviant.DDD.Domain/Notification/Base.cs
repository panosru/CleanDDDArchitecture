namespace Aviant.DDD.Domain.Notification
{
    public abstract class Base : INotification
    {
        public string? Message { get; private set; }

        protected Base()
        {
            Message = null;
        }

        protected Base(string message)
        {
            Message = message;
        }
    }
}