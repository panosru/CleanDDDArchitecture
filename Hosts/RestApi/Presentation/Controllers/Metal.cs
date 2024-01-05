using System.Diagnostics;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Controllers;

/// <inheritdoc />
/// <summary>
///     Metal endpoint
/// </summary>
[ApiVersion("1.0")]
[ApiVersion("1.1")]
[AllowAnonymous]
[FeatureGate(Core.Features.Features.Metal)]
public sealed class Metal : ApiController
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
        var assembly = typeof(Program).Assembly;

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