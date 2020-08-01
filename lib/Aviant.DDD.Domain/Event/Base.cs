using System;

namespace Aviant.DDD.Domain.Event
{
    public abstract class Base : IEvent, INotification
    {
        public DateTime Occured { get; protected set; } = DateTime.Now;
    }
}