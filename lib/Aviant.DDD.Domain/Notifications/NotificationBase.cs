namespace Aviant.DDD.Domain.Notifications
{
    public abstract class NotificationBase : INotification
    {
        protected NotificationBase()
        {
            Message = null;
        }

        protected NotificationBase(string message)
        {
            Message = message;
        }

        public string? Message { get; }
    }
}