namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_1.Forecast
{
    using System.Net.Mime;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Weather.Application.UseCases.ForecastV1_1;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [FeatureGate(Features.WeatherForecastV11)]
    public sealed class Weather
        : ApiController<ForecastUseCase, Weather>,
          IForecastOutput
    {
        /// <inheritdoc />
        public Weather([FromServices] ForecastUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IForecastOutput Members

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void IForecastOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        /// <summary>
        /// </summary>
        /// <param name="object"></param>
        void IForecastOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion

        /// <summary>
        ///     Performs weather forecast
        /// </summary>
        /// <response code="200">Todo list already exists</response>
        /// <response code="404">Not found.</response>
        /// <returns>The forecast for the next days</returns>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Get))]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Forecast()
        {
            await UseCase.ExecuteAsync()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}