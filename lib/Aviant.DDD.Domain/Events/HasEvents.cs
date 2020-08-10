namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public abstract class HasEvents
    {
        public List<EventBase> Events = new List<EventBase>();
    }
}