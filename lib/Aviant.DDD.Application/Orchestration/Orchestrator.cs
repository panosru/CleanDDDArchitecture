namespace Aviant.DDD.Application.Orchestration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Domain.Events;
    using Domain.Persistence;
    using MediatR;

    public class Orchestrator : IOrchestrator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IMediator _mediator;

        public Orchestrator(IUnitOfWork unitOfWork, IEventDispatcher eventDispatcher, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _eventDispatcher = eventDispatcher;
            _mediator = mediator;
        }

        public async Task<RequestResult> SendCommand<T>(ICommand<T> command)
        {
            var commandResponse = await _mediator.Send(command);
            
            // Fire pre/post events
            await _eventDispatcher.FirePreCommitEvents();

            if (await _unitOfWork.Commit())
            {
                return new RequestResult
                {
                    Success = true,
                    Payload = commandResponse
                };
            }
            else
            {
                return new RequestResult
                {
                    Success = false,
                    Messages = new List<string>
                    {
                        "An error occurred"
                    }
                };
            }
        }

        public Task<RequestResult> SendQuery<T>(ICommand<T> command)
        {
            throw new System.NotImplementedException();
        }
    }
}