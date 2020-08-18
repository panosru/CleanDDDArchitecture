namespace CleanDDDArchitecture.RestApi.Controllers
{
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
        private IOrchestrator? _orchestrator;

        /// <summary>
        /// 
        /// </summary>
        protected IOrchestrator Orchestrator => _orchestrator ??= HttpContext.RequestServices.GetService<IOrchestrator>();
    }
}