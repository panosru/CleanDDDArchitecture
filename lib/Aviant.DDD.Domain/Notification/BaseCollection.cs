namespace Aviant.DDD.Domain.Notification
{
    using System.Collections.Generic;

    public abstract class BaseCollection : INotifications
    {
        private List<INotification> _notifications;

        public BaseCollection()
        {
            _notifications = new List<INotification>();
        }

        public void AddNotification(INotification notification)
        {
            _notifications.Add(notification);
        }

        public void CleanNotifications()
        {
            _notifications = new List<INotification>();
        }

        public List<INotification> GetAll()
        {
            return _notifications;
        }

        public bool HasNotifications()
        {
            return 0 < _notifications.Count;
        }
    }
}