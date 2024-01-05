using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.EventSourcing.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation;

/// <inheritdoc />
/// <summary>
///     Account endpoints
/// </summary>
[RouteSegment(IAccountDomainConfiguration.RouteSegment)] // If not present, the Domain name will be used e.g. "account"
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
[RouteSegment(IAccountDomainConfiguration.RouteSegment)]
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
