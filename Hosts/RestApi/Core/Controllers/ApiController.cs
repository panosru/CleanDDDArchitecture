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

    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController<TUseCase> : ControllerBase
        where TUseCase : class, IUseCase
    {
        protected readonly TUseCase UseCase;
        
        protected IActionResult ViewModel { get; set; } = new NoContentResult();

        protected ApiController(TUseCase useCase)
        {
            UseCase = useCase;
        }
    }
}