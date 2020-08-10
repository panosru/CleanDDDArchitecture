namespace Aviant.DDD.Domain.Notifications
{
    using System.Collections.Generic;

    public interface INotifications
    {
        void AddNotification(INotification notification);

        bool HasNotifications();

        List<INotification> GetAll();

        void CleanNotifications();
    }
}