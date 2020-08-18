namespace Aviant.DDD.Application.Orchestration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Domain.Events;
    using Domain.Notifications;
    using Domain.Persistence;
    using MediatR;

    public class Orchestrator : IOrchestrator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotifications _notifications;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMediator _mediator;

        public Orchestrator(
            IUnitOfWork unitOfWork, 
            INotifications notifications,
            IEventDispatcher eventDispatcher,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _notifications = notifications;
            _eventDispatcher = eventDispatcher;
            _mediator = mediator;
        }

        public async Task<RequestResult> SendCommand<T>(ICommand<T> command)
        {
            var commandResponse = await _mediator.Send(command);
            
            // Fire pre/post events
            await _eventDispatcher.FirePreCommitEvents();

            if (_notifications.HasNotifications())
            {
                return new RequestResult(_notifications.GetAll());
            }

            if (await _unitOfWork.Commit())
            {
                // Fire post commit events
                await _eventDispatcher.FirePostCommitEvents();

                bool isLazy = false;
                
                try
                {
                    isLazy = typeof(Lazy<>) == (
                        commandResponse?.GetType().GetGenericTypeDefinition());
                }
                catch (Exception e)
                {
                    // ignored
                }

                return new RequestResult(
                    isLazy
                        ? commandResponse?.GetType().GetProperty("Value")?.GetValue(commandResponse, null)
                        : commandResponse);
            }

            return new RequestResult(new List<string>
            {
                "An error occurred"
            });
        }

        public async Task<RequestResult> SendQuery<T>(ICommand<T> command)
        {
            var commandResponse = await _mediator.Send(command);

            if (_notifications.HasNotifications())
            {
                return new RequestResult(_notifications.GetAll());
            }
            
            return new RequestResult(commandResponse);
        }
    }
}