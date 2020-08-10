namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;

    public interface IEvents
    {
        void AddEvent(IEvent @event);

        bool HasEvents();

        List<IEvent> GetAll();

        void CleanEvents();
    }
}