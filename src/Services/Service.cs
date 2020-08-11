namespace CleanDDDArchitecture.Services
{
    using MediatR;

    public class BaseService //TODO: Once Aggregates will be ready then I'll revisit this project 
    {
        protected BaseService(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IMediator Mediator { get; set; }
    }
}