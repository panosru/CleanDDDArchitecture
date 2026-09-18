using Aviant.Presentation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core.Errors;

/// <summary>
///     Registers and enables the single error pipeline shared by every host.
/// </summary>
public static class ApiErrorHandlingExtensions
{
    /// <summary>Adds problem details, <see cref="AviantExceptionHandler" /> and <see cref="UnexpectedExceptionHandler" />.</summary>
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        // Handlers run in order until one answers: Aviant's maps validation failures,
        // missing resources and refused domain rules; anything else is a logged 500.
        services.AddAviantProblemDetails();
        services.AddExceptionHandler<UnexpectedExceptionHandler>();

        return services;
    }

    /// <summary>Routes unhandled exceptions through the registered handlers.</summary>
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        return app;
    }
}
