namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers
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
        /// <summary>
        /// </summary>
        protected IOrchestrator Orchestrator => 
            HttpContext.RequestServices.GetRequiredService<IOrchestrator>();
    }
}