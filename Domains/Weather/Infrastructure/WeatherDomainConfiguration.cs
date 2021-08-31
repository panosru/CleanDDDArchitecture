namespace CleanDDDArchitecture.Domains.Weather.Infrastructure
{
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Microsoft.Extensions.Configuration;

    public sealed class WeatherDomainConfiguration : DomainConfigurationContainer
    {
        /// <inheritdoc />
        public WeatherDomainConfiguration(IConfiguration configuration)
            : base(configuration)
        { }
    }
}