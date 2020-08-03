namespace Aviant.DDD.Domain.Notification
{
    public abstract class Base : INotification
    {
        protected Base()
        {
            Message = null;
        }

        protected Base(string message)
        {
            Message = message;
        }

        public string? Message { get; }
    }
}