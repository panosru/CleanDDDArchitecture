using System.Collections.Generic;

namespace Aviant.DDD.Domain
{
    public interface INotifications
    {
        void AddNotification(INotification notification);

        bool HasNotifications();

        List<INotification> GetAll();

        void CleanNotifications();
    }
}