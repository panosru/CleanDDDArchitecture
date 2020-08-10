namespace Aviant.DDD.Application.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Events;
    using MediatR;

    public abstract class EventDispatcherBase : IEventDispatcher
    {
        private readonly IMediator _mediator;

        public EventDispatcherBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        private List<IEvent> PreCommitEvents { get; } = new List<IEvent>();
        private List<IEvent> PostCommitEvents { get; } = new List<IEvent>();

        public void AddPreCommitEvent(IEvent @event)
        {
            PreCommitEvents.Add(@event);
        }

        public void AddPostCommitEvent(IEvent @event)
        {
            PostCommitEvents.Add(@event);
        }

        public async Task FirePreCommitEvents()
        {
            foreach (IEvent @event in PreCommitEvents)
                await _mediator.Publish(@event);
        }

        public async Task FirePostCommitEvents()
        {
            foreach (IEvent @event in PostCommitEvents)
                await _mediator.Publish(@event);
        }

        public List<IEvent> GetPreCommitEvents()
        {
            return PreCommitEvents;
        }

        public List<IEvent> GetPostCommitEvents()
        {
            return PostCommitEvents;
        }

        public void RemovePreCommitEvent(IEvent @event)
        {
            PreCommitEvents.Remove(@event);
        }

        public void RemovePostCommitEvent(IEvent @event)
        {
            PostCommitEvents.Remove(@event);
        }
    }
}