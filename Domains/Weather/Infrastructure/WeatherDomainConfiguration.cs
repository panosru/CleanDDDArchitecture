using Aviant.Infrastructure.DDD.CrossCutting;
using Microsoft.Extensions.Configuration;

namespace CleanDDDArchitecture.Domains.Weather.Infrastructure;

public sealed class WeatherDomainConfiguration : DomainConfigurationContainer
{
    /// <inheritdoc />
    public WeatherDomainConfiguration(IConfiguration configuration)
        : base(configuration)
    { }
}
