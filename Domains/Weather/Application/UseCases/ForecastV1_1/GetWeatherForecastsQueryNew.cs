namespace CleanDDDArchitecture.Domains.Weather.Application.UseCases.ForecastV1_1;

using Aviant.DDD.Application.Queries;
using Polly;
using Services;

internal sealed class GetWeatherForecastsQueryNew : Query<IEnumerable<WeatherForecastService>>
{
    #region Nested type: GetWeatherForecastsQueryNewHandler

    internal sealed class
        GetWeatherForecastsQueryNewHandler
        : QueryHandler<GetWeatherForecastsQueryNew, IEnumerable<WeatherForecastService>>
    {
        private static readonly string[] Summaries =
        {
            "Freezing v1.1", "Bracing v1.1", "Chilly v1.1", "Cool v1.1", "Mild v1.1",
            "Warm v1.1", "Balmy v1.1", "Hot v1.1", "Sweltering v1.1", "Scorching v1.1"
        };

        public override Task<IEnumerable<WeatherForecastService>> Handle(
            GetWeatherForecastsQueryNew request,
            CancellationToken           cancellationToken)
        {
            Random rng = new();

            // 40% probability to fail
            if (rng.Next(100) <= 40)
                throw new Exception("Something gone really wrong...");

            IEnumerable<WeatherForecastService> vm = Enumerable.Range(1, 5)
               .Select(
                    index => new WeatherForecastService(
                        DateTime.Now.AddDays(index),
                        rng.Next(-20, 55),
                        Summaries[rng.Next(Summaries.Length)]));

            return Task.FromResult(vm);
        }

        public override IAsyncPolicy RetryPolicy() =>
            Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(
                    2,
                    i => TimeSpan.FromSeconds(i));
    }

    #endregion
}
