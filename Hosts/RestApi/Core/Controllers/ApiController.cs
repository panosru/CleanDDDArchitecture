namespace CleanDDDArchitecture.Hosts.RestApi.Core.Controllers
{
    #region

    using Aviant.DDD.Application.Orchestration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

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