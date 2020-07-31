using MediatR;

namespace CleanArchitecture.Services
{
    public class BaseService
    {
        protected IMediator _mediator { get; set; }

        protected BaseService(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}