﻿namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.SyncWeatherService
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Weather.Application.UseCases.SyncWeatherService;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    [AllowAnonymous]
    [FeatureGate(Features.WeatherSyncService)]
    public class Weather
        : ApiController<SyncWeatherServiceUseCase, Weather>, ISyncWeatherServiceOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Weather([FromServices] SyncWeatherServiceUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region ISyncWeatherServiceOutput Members

        void ISyncWeatherServiceOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ISyncWeatherServiceOutput.NoContent() => ViewModel = NoContent();

        #endregion

        /// <summary>
        ///     Dummy weather syncing with external service (3 seconds delay)
        /// </summary>
        /// <response code="200">Synchornisation successfully comleted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="command"></param>
        /// <returns>Successful message.</returns>
        [HttpPost]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Patch))]
        public async Task<IActionResult> Forecast([FromBody] SyncWeatherServiceCommand command)
        {
            await UseCase
               .SetInput(command)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}