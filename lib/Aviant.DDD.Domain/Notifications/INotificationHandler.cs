namespace Aviant.DDD.Domain.Notifications
{
    using MediatR;

    public interface INotificationHandler<in TNotification> : MediatR.INotificationHandler<TNotification>
        where TNotification : Notifications.INotification
    {
    }
}