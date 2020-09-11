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
        private IOrchestrator? _orchestrator;

        /// <summary>
        /// </summary>
        protected IOrchestrator Orchestrator =>
            _orchestrator ??= HttpContext.RequestServices.GetService<IOrchestrator>();
    }
}