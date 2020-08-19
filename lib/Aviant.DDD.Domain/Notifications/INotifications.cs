namespace Aviant.DDD.Domain.Notifications
{
    using System.Collections.Generic;

    public interface INotifications
    {
        void AddNotification(string notification);

        bool HasNotifications();

        List<string> GetAll();

        void CleanNotifications();
    }
}