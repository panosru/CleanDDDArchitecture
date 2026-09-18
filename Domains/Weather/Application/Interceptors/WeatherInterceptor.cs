using Aviant.Application.Interceptors;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Weather.Application.Interceptors;

public sealed class WeatherInterceptor(ILogger<WeatherInterceptor> logger) : InterceptorBase<WeatherInterceptor>
{
    /// <inheritdoc />
    protected override void OnPre(InterceptorContext context)
    {
        logger.LogInformation("Before weather service");
    }

    /// <inheritdoc />
    protected override void OnPost(InterceptorContext context)
    {
        logger.LogInformation("After weather service");
    }

    /// <inheritdoc />
    protected override void OnExit(InterceptorContext context)
    {
        logger.LogInformation("Exiting weather service");
    }
}
