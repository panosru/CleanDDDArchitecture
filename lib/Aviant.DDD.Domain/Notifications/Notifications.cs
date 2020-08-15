namespace Aviant.DDD.Domain.Notifications
{
    using System.Collections.Generic;

    public class Notifications : INotifications
    {
        private List<string> _notifications = new List<string>();

        public void AddNotification(string notification)
        {
            _notifications.Add(notification);
        }

        public void CleanNotifications()
        {
            _notifications = new List<string>();
        }

        public List<string> GetAll()
        {
            return _notifications;
        }

        public bool HasNotifications()
        {
            return 0 < _notifications.Count;
        }
    }
}