namespace CleanDDDArchitecture.RestApi.Controllers
{
    using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OidcConfiguration : Controller
    {
        private readonly ILogger<OidcConfiguration> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientRequestParametersProvider"></param>
        /// <param name="logger"></param>
        public OidcConfiguration(IClientRequestParametersProvider clientRequestParametersProvider,
            ILogger<OidcConfiguration> logger)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet("_configuration/{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);
        }
    }
}