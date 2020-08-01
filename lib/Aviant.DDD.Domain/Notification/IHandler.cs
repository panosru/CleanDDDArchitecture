using MediatR;

namespace Aviant.DDD.Domain.Notification
{
    public interface IHandler<in TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
    }
}