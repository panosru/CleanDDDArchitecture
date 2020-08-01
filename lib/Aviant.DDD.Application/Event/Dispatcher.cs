using System.Collections.Generic;
using System.Threading.Tasks;
using Aviant.DDD.Domain.Event;
using Aviant.DDD.Domain.TransferObject;
using MediatR;
using INotification = Aviant.DDD.Domain.INotification;

namespace Aviant.DDD.Application.Event
{
    public abstract class DispatcherBase : IDispatcher
    {
        private readonly IMediator _mediator;
        
        private List<INotification> PreCommitEvents { get; set; } = new List<INotification>();
        private List<INotification> PostCommitEvents { get; set; } = new List<INotification>();

        public DispatcherBase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void AddPreCommitEvent(INotification evnt)
        {
            PreCommitEvents.Add(evnt);
        }

        public void AddPostCommitEvent(INotification evnt)
        {
            PostCommitEvents.Add(evnt);
        }

        public async Task FirePreCommitEvents()
        {
            foreach (INotification evnt in PreCommitEvents)
                await _mediator.Publish(evnt);
        }

        public async Task FirePostCommitEvents()
        {
            foreach (INotification evnt in PostCommitEvents)
                await _mediator.Publish(evnt);
        }

        public List<INotification> GetPreCommitEvents()
        {
            return PreCommitEvents;
        }

        public List<INotification> GetPostCommitEvents()
        {
            return PostCommitEvents;
        }

        public void RemovePreCommitEvent(INotification evnt)
        {
            PreCommitEvents.Remove(evnt);
        }

        public void RemovePostCommitEvent(INotification evnt)
        {
            PostCommitEvents.Remove(evnt);
        }
    }
}