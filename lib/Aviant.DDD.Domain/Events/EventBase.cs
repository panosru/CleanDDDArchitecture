namespace Aviant.DDD.Domain.Events
{
    using System;
    using TransferObjects;

    public abstract class EventBase : IEvent
    {
        public DateTime Occured { get; protected set; } = DateTime.Now;
        
        public string? Message { get; }

        protected EventBase()
        {
            Message = null;
        }

        protected EventBase(string message)
        {
            Message = message;
        }
    }
}