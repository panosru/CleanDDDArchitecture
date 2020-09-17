namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_1.Forecast
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.ForecastV1_1;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController, IForecastOutput
    {
        private readonly ForecastUseCase _useCase;

        public Weather([FromServices] ForecastUseCase useCase) => _useCase = useCase;

        private IActionResult ViewModel { get; set; } = new NoContentResult();

        #region IForecastOutput Members

        void IForecastOutput.Invalid(string message)
        {
            ViewModel = BadRequest(message);
        }

        void IForecastOutput.Ok(object? @object) => ViewModel = Ok(@object);

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public Task<IActionResult> Forecast()
        {
            _useCase.Execute(this);

            return Task.FromResult(ViewModel);
        }
    }
}