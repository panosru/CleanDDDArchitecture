namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OpenApi.Models;

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
    }
}