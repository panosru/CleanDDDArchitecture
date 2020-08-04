namespace CleanDDDArchitecture.Services
{
    using MediatR;

    public class BaseService
    {
        protected BaseService(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IMediator _mediator { get; set; }
    }
}