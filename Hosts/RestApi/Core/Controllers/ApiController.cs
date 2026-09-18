using Aviant.Application.UseCases;
using Aviant.Presentation.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;

// Every API controller routes as api/{domain segment}/...; see RouteSegmentAttribute.
[Route("api/[segments]")]
public abstract class ApiController : OrchestratorController;

[Route("api/[segments]")]
public abstract class ApiController<TUseCase, TUseCaseOutput>(TUseCase useCase)
    : UseCaseController<TUseCase, TUseCaseOutput>(useCase)
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput;
