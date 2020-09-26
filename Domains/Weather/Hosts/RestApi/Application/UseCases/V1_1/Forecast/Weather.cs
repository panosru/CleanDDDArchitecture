namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_1.Forecast
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using Domains.Weather.Application.UseCases.ForecastV1_1;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    [AllowAnonymous]
    public class Weather
        : ApiController<ForecastUseCase, Weather>,
          IForecastOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Weather([FromServices] ForecastUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IForecastOutput Members

        void IForecastOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

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
            await UseCase.Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}