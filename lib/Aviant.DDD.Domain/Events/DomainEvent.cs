namespace Aviant.DDD.Domain.Events
{
    public class DomainEvent : EventBase
    {
        public DomainEvent(string message) : base(message)
        {
        }
    }
}