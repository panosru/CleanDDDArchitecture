namespace Aviant.DDD.Application.Events
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Events;
    using MediatR;

    public class EventDispatcher : IEventDispatcher 
    {
        private readonly IMediator _mediator;

        public EventDispatcher(IMediator mediator)
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
            foreach (IEvent @event in PreCommitEvents.ToList())
            {
                await _mediator.Publish(@event).ConfigureAwait(false);
                RemovePreCommitEvent(@event);
            }
        }

        public async Task FirePostCommitEvents()
        {
            foreach (IEvent @event in PostCommitEvents.ToList())
            {
                await _mediator.Publish(@event).ConfigureAwait(false);
                RemovePostCommitEvent(@event);
            }
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