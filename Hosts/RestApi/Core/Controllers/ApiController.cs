namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers
{
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
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

    /// <summary>
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
        
        protected IActionResult ViewModel { get; set; } = new NoContentResult();

        protected ApiController(TUseCase useCase)
        {
            UseCase = useCase;
        }
    }
}