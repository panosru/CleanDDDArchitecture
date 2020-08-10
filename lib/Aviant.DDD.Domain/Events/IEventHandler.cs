namespace Aviant.DDD.Domain.Events
{
    using MediatR;

    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
    }
}