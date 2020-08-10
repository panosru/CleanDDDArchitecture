namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public abstract class HasEvents
    {
        public List<IEvent> Events = new List<IEvent>();
    }
}