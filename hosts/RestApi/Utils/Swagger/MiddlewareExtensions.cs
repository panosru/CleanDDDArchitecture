namespace CleanArchitecture.RestApi.Utils.Swagger
{
    using System;
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    ///     Extending Swagger services
    /// </summary>
    public static class MiddlewareExtensions
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