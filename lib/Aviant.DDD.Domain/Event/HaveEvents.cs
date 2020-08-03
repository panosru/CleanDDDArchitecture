namespace Aviant.DDD.Domain.Event
{
    using System.Collections.Generic;

    public abstract class HaveEvents
    {
        public List<Base> Events = new List<Base>();
    }
}