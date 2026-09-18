using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;

/// <summary>
///     Registers and enables the single error pipeline shared by every host.
/// </summary>
public static class ApiErrorHandlingExtensions
{
    /// <summary>Adds problem details and <see cref="ProblemDetailsExceptionHandler" />.</summary>
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        return services;
    }

    /// <summary>Routes unhandled exceptions through <see cref="ProblemDetailsExceptionHandler" />.</summary>
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        return app;
    }
}
