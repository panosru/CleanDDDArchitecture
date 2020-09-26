namespace CleanDDDArchitecture.Hosts.RestApi.Application.Controllers
{
    using System.Diagnostics;
    using Core;
    using Core.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Metal endpoint
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [AllowAnonymous]
    public class Metal : ApiController
    {
        /// <summary>
        ///     Metal info
        /// </summary>
        /// <response code="200">Metal data.</response>
        /// <response code="404">Not Found.</response>
        /// <returns>Metal info data.</returns>
        [HttpGet("/metal")]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
        public ActionResult<object> Get()
        {
            var assembly = typeof(Startup).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version      = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok(
                new
                {
                    Version     = version,
                    LastUpdated = creationDate
                });
        }
    }
}