namespace Aviant.DDD.Domain.Events
{
    using System;
    using MediatR;

    public interface IEvent : INotification
    {
        public DateTime Occured { get; set; }
    }
}