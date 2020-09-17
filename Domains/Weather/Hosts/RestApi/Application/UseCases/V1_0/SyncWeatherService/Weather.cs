namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.SyncWeatherService
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.SyncWeatherService;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController, ISyncWeatherServiceOutput
    {
        private readonly SyncWeatherServiceUseCase _useCase;

        public Weather([FromServices] SyncWeatherServiceUseCase useCase) => _useCase = useCase;

        private IActionResult ViewModel { get; set; } = new NoContentResult();

        #region ISyncWeatherServiceOutput Members

        void ISyncWeatherServiceOutput.Invalid(string message)
        {
            ViewModel = BadRequest(message);
        }

        void ISyncWeatherServiceOutput.NoContent() => ViewModel = NoContent();

        #endregion

        [HttpPost]
        [AllowAnonymous]
        public Task<IActionResult> Forecast([FromBody] SyncWeatherServiceCommand command)
        {
            _useCase.Execute(this, command);

            return Task.FromResult(ViewModel);
        }
    }
}