// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

/// <summary>
///     Swagger Configuration
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class SwaggerSettings
{
    /// <summary>
    ///     Gets or sets document Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets swagger Info.
    /// </summary>
    public OpenApiInfo? Info { get; set; }

    /// <summary>
    ///     Gets or sets RoutePrefix.
    /// </summary>
    public string? RoutePrefix { get; set; }
    
    /// <summary>
    ///    Gets or sets VersionInfo.
    /// </summary>
    public Dictionary<string, OpenApiInfo> VersionInfo { get; set; }
}
