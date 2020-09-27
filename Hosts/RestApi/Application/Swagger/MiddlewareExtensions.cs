namespace CleanDDDArchitecture.Hosts.RestApi.Application.Swagger
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    ///     Extending Swagger services
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class MiddlewareExtensions
    {
        /// <summary>
        ///     Enabling Swagger UI.
        ///     Excluding from test environment
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        public static void UseSwaggerDocuments(this IApplicationBuilder app)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "TEST") return;

            app.UseSwagger();

            app.UseSwaggerUI();
        }
    }
}