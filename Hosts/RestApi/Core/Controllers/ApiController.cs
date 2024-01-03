using Aviant.Application.Orchestration;
using Aviant.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;

/// <inheritdoc />
/// <summary>
///     API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// </summary>
    protected IOrchestrator Orchestrator =>
        HttpContext.RequestServices.GetRequiredService<IOrchestrator>();
}

/// <inheritdoc />
/// <summary>
///     API Controller
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController<TUseCase, TUseCaseOutput> : ControllerBase
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput
{
    protected readonly TUseCase UseCase;

    protected ApiController(TUseCase useCase) => UseCase = useCase;

    protected IActionResult ViewModel { get; set; } = new NoContentResult();
}
