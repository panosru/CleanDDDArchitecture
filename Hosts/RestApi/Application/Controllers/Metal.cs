namespace CleanDDDArchitecture.Hosts.RestApi.Application.Controllers
{
    #region

    using System.Diagnostics;
    using Core.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    #endregion

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
        /// <returns></returns>
        [HttpGet("/metal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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