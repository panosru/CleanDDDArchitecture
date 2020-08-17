namespace Aviant.DDD.Domain.Events
{
    using System;

    public abstract class EventBase : IEvent
    {
        public DateTime Occured { get; set; }
    }
}