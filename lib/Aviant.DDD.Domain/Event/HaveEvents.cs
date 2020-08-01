using System.Collections.Generic;

namespace Aviant.DDD.Domain.Event
{
    public abstract class HaveEvents
    {
        public List<Event.Base> Events = new List<Event.Base>();
    }
}