namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public abstract class HaveEvents : IHaveEvents
    {
        private List<EventBase> _events = new List<EventBase>();
        
        public void AddEvent(EventBase @event)
        {
            _events.Add(@event);
        }

        public bool HasEvents()
        {
            return 0 < _events.Count;
        }

        public List<EventBase> GetAll()
        {
            return _events;
        }

        public void CleanEvents()
        {
            _events = new List<EventBase>();
        }
    }
}