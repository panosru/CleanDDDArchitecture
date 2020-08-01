using System.Threading;
using System.Threading.Tasks;

namespace Aviant.DDD.Domain.Notification
{
    public abstract class Handler<TNotification> : IHandler<TNotification>
        where TNotification : INotification
    {
        public abstract Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}