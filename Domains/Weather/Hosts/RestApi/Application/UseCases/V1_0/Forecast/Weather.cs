namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.Forecast
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.Forecast;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController<ForecastUseCase>, IForecastOutput
    {
        public Weather([FromServices] ForecastUseCase useCase)
            : base(useCase)
        { }

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
            await UseCase.ExecuteAsync(this);

            return ViewModel;
        }
    }
}