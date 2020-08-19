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
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMediator _mediator;
        private readonly INotifications _notifications;
        private readonly IUnitOfWork _unitOfWork;

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

            if (_notifications.HasNotifications()) return new RequestResult(_notifications.GetAll());

            var affectedRows = await _unitOfWork.Commit();

            if (-1 == affectedRows)
                return new RequestResult(
                    new List<string>
                    {
                        "An error occurred"
                    });

            // Fire post commit events
            await _eventDispatcher.FirePostCommitEvents();

            var isLazy = false;

            try
            {
                isLazy = typeof(Lazy<>) == commandResponse?.GetType().GetGenericTypeDefinition();
            }
            catch (Exception)
            {
                // ignored
            }

            return new RequestResult(
                isLazy
                    ? commandResponse?.GetType().GetProperty("Value")?.GetValue(commandResponse, null)
                    : commandResponse,
                affectedRows);
        }

        public async Task<RequestResult> SendQuery<T>(ICommand<T> command)
        {
            var commandResponse = await _mediator.Send(command);

            if (_notifications.HasNotifications()) return new RequestResult(_notifications.GetAll());

            return new RequestResult(commandResponse);
        }
    }
}