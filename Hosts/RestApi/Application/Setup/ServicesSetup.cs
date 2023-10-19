using Aviant.Core.Timing;

namespace CleanDDDArchitecture.Hosts.RestApi.Application.Setup;

/// <summary>
///  Services setup
/// </summary>
public class ServicesSetup
{
    /// <summary>
    ///   Setup services
    /// </summary>
    /// <param name="services"></param>
    public void Setup(IServiceCollection services)
    {
        Clock.Provider = ClockProviders.Utc;
    }
}
