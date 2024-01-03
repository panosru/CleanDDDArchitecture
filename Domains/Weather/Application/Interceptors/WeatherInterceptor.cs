using Aviant.Application.Interceptors;
using Serilog;

namespace CleanDDDArchitecture.Domains.Weather.Application.Interceptors;

public sealed class WeatherInterceptor : InterceptorBase<WeatherInterceptor>
{
    /// <inheritdoc />
    protected override void OnPre(InterceptorContext context)
    {
        Log.Information("Before weather service");
    }

    /// <inheritdoc />
    protected override void OnPost(InterceptorContext context)
    {
        Log.Information("After weather service");
    }

    /// <inheritdoc />
    protected override void OnExit(InterceptorContext context)
    {
        Log.Information("Exiting weather service");
    }
}
