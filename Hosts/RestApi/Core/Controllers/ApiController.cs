using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;

/// <inheritdoc />
/// <summary>
/// API Shared Controller
/// </summary>
[ApiController]
[Route("api/[segments]")]
public abstract class ApiSharedController : ControllerBase;

/// <inheritdoc />
/// <summary>
/// API Controller
/// </summary>
public abstract class ApiController : ApiSharedController
{
    /// <summary>
    /// </summary>
    protected IOrchestrator Orchestrator =>
        HttpContext.RequestServices.GetRequiredService<IOrchestrator>();
}

/// <inheritdoc />
/// <summary>
/// API Controller Generic
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
public abstract class ApiController<TUseCase, TUseCaseOutput> : ApiSharedController
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput
{
    protected readonly TUseCase UseCase;

    protected ApiController(TUseCase useCase) => UseCase = useCase;

    protected IActionResult ViewModel { get; set; } = new NoContentResult();
}
