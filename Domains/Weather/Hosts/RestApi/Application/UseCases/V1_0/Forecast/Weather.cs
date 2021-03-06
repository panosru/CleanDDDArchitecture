﻿namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.Forecast
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Weather.Application.UseCases.Forecast;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [AllowAnonymous]
    [FeatureGate(Features.WeatherForecast)]
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
        public async Task<IActionResult> Forecast()
        {
            await UseCase.ExecuteAsync()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}