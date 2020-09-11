namespace CleanDDDArchitecture.Domains.Weather.CrossCutting
{
    #region

    using System.Collections.Generic;
    using System.Reflection;
    using Application.UseCases.Forecast;
    using AutoMapper;

    #endregion

    public static class WeatherCrossCutting
    {
        public static IEnumerable<Profile> WeatherAutoMapperProfiles() => new List<Profile>();

        public static IEnumerable<Assembly> WeatherValidatorAssemblies() => new List<Assembly>();

        public static IEnumerable<Assembly> WeatherMediatorAssemblies() => new List<Assembly>
        {
            typeof(GetWeatherForecastsQuery).Assembly
        };
    }
}