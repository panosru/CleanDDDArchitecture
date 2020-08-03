namespace Aviant.DDD.Domain.Notification
{
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class Handler<TNotification> : IHandler<TNotification>
        where TNotification : INotification
    {
        public abstract Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}