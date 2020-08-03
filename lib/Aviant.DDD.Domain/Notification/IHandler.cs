namespace Aviant.DDD.Domain.Notification
{
    using MediatR;
    using INotification = Domain.INotification;

    public interface IHandler<in TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
    }
}