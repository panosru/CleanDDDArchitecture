namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application;

using Account.Application.Aggregates;
using Aviant.Application.UseCases;
using Aviant.EventSourcing.Application.Orchestration;
using Microsoft.Extensions.DependencyInjection;

/// <inheritdoc />
/// <summary>
///     Account endpoints
/// </summary>
public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
{
    /// <summary>
    /// </summary>
    protected new IOrchestrator<AccountAggregate, AccountAggregateId> Orchestrator =>
        HttpContext.RequestServices.GetRequiredService<IOrchestrator<AccountAggregate, AccountAggregateId>>();
}

/// <inheritdoc />
/// <summary>
///     Account endpoints
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
public abstract class ApiController<TUseCase, TUseCaseOutput>
    : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase, TUseCaseOutput>
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput
{
    /// <inheritdoc />
    protected ApiController(TUseCase useCase)
        : base(useCase)
    { }
}
