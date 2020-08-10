namespace Aviant.DDD.Domain.Notifications
{
    public class DomainNotification : NotificationBase
    {
        public DomainNotification()
        {
        }

        public DomainNotification(string message) : base(message)
        {
        }
    }
}