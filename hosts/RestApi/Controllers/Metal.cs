namespace CleanDDDArchitecture.RestApi.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// </summary>
    public class Metal : ApiController
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet("/metal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> Get()
        {
            var assembly = typeof(Startup).Assembly;

            var creationDate = System.IO.File.GetCreationTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

            return Ok(
                new
                {
                    Version = version,
                    LastUdated = creationDate
                });
        }
    }
}