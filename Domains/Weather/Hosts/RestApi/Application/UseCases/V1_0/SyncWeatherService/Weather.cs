﻿namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.SyncWeatherService
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.SyncWeatherService;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController<SyncWeatherServiceUseCase>, ISyncWeatherServiceOutput
    {
        public Weather([FromServices] SyncWeatherServiceUseCase useCase)
            : base(useCase)
        { }

        #region ISyncWeatherServiceOutput Members

        void ISyncWeatherServiceOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ISyncWeatherServiceOutput.NoContent() => ViewModel = NoContent();

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Forecast([FromBody] SyncWeatherServiceCommand command)
        {
            await UseCase.ExecuteAsync(this, command);

            return ViewModel;
        }
    }
}