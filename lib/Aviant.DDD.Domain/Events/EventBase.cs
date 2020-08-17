namespace Aviant.DDD.Domain.Events
{
    using System;

    public abstract class EventBase : IEvent
    {
        public DateTime Occured { get; protected set; } = DateTime.Now; //TODO: Use DateTimeService instead
    }
}