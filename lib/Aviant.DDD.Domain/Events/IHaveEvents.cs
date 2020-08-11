namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public interface IHaveEvents 
    {
        void AddEvent(EventBase @event);

        bool HasEvents();

        List<EventBase> GetAll();

        void CleanEvents();
    }
}