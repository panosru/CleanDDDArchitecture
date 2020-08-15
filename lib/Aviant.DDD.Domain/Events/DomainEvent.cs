namespace Aviant.DDD.Domain.Events
{
    public class DomainEvent : EventBase
    {
        public DomainEvent(string message) //TODO: Check projects for Domain Events
            : base(message)
        {
        }
    }
}