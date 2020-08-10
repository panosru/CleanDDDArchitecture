namespace Aviant.DDD.Domain.Events
{
    using System;
    using Notifications;

    public abstract class EventBase : IEvent, INotification
    {
        public DateTime Occured { get; protected set; } = DateTime.Now;
    }
}