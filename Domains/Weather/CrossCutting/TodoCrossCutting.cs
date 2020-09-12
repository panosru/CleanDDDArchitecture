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
        public static IEnumerable<Profile> AutoMapperProfiles() => new List<Profile>();

        public static IEnumerable<Assembly> ValidatorAssemblies() => new List<Assembly>();

        public static IEnumerable<Assembly> MediatorAssemblies() => new List<Assembly>
        {
            typeof(GetWeatherForecastsQuery).Assembly
        };
    }
}