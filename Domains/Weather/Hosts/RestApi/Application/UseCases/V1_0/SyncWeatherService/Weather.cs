namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.SyncWeatherService
{
    using System.Threading;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Weather.Application.UseCases.SyncWeatherService;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [AllowAnonymous]
    [FeatureGate(Features.WeatherSyncService)]
    public sealed class Weather
        : ApiController<SyncWeatherServiceUseCase, Weather>, ISyncWeatherServiceOutput
    {
        /// <inheritdoc />
        public Weather([FromServices] SyncWeatherServiceUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ISyncWeatherServiceOutput Members

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void ISyncWeatherServiceOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        /// <summary>
        /// </summary>
        void ISyncWeatherServiceOutput.NoContent() => ViewModel = NoContent();

        #endregion

        /// <summary>
        ///     Dummy weather syncing with external service (3 seconds delay)
        /// </summary>
        /// <response code="200">Synchronization successfully completed.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="cancellationToken"></param>
        /// <returns>Successful message.</returns>
        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Patch))]
        public async Task<IActionResult> Forecast(CancellationToken cancellationToken = default)
        {
            await UseCase.ExecuteAsync(cancellationToken)
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}