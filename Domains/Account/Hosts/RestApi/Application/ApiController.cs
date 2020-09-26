namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application
{
    using Account.Application.Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    {
        protected new IOrchestrator<AccountAggregate, AccountAggregateId> Orchestrator =>
            HttpContext.RequestServices.GetRequiredService<IOrchestrator<AccountAggregate, AccountAggregateId>>();
    }

    public class ApiController<TUseCase, TUseCaseOutput>
        : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase, TUseCaseOutput>
        where TUseCase : class, IUseCase<TUseCaseOutput>
        where TUseCaseOutput : class, IUseCaseOutput
    {
        public ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}