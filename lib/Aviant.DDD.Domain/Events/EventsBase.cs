namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public abstract class EventsBase :
        IEvents
    {
        private List<IEvent> _events;

        public EventsBase()
        {
            _events = new List<IEvent>();
        }

        public void AddEvent(IEvent @event)
        {
            _events.Add(@event);
        }

        public bool HasEvents()
        {
            return 0 < _events.Count;
        }

        public List<IEvent> GetAll()
        {
            return _events;
        }

        public void CleanEvents()
        {
            _events = new List<IEvent>();
        }
    }
}