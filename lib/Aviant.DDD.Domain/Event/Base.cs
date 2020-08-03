namespace Aviant.DDD.Domain.Event
{
    using System;

    public abstract class Base : IEvent, INotification
    {
        public DateTime Occured { get; protected set; } = DateTime.Now;
    }
}