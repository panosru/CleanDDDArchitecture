namespace Aviant.DDD.Domain.Events
{
    using System;

    public abstract class EventBase : IEvent
    {
        protected EventBase()
        {
            Message = null;
        }

        protected EventBase(string message)
        {
            Message = message;
        }

        public DateTime Occured { get; protected set; } = DateTime.Now; //TODO: Use DateTimeService instead

        public string? Message { get; }
    }
}