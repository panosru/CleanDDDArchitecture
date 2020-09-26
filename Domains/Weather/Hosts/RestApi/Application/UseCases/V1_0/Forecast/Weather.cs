namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.Forecast
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.Forecast;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather
        : ApiController<ForecastUseCase, Weather>,
          IForecastOutput
    {
        public Weather([FromServices] ForecastUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IForecastOutput Members

        void IForecastOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IForecastOutput.Ok(object? @object) =>
            ViewModel = Ok(@object);

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Forecast()
        {
            await UseCase.Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}