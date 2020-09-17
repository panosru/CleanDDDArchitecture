namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.AddCity
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.AddCity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController, IAddCityOutput
    {
        private readonly AddCityUseCase _useCase;

        public Weather([FromServices] AddCityUseCase useCase) => _useCase = useCase;

        private IActionResult ViewModel { get; set; } = new NoContentResult();

        #region IAddCityOutput Members

        void IAddCityOutput.Invalid(string message)
        {
            ViewModel = BadRequest(message);
        }

        void IAddCityOutput.Ok(string city) =>
            ViewModel = Ok((new AddCityResponse(city)).ToString());

        void IAddCityOutput.NotFound() => ViewModel = NotFound();

        #endregion

        [HttpPost("[action]")]
        [AllowAnonymous]
        public Task<IActionResult> AddCity([FromBody] AddCityDto dto)
        {
            _useCase.Execute(this, dto);

            return Task.FromResult(ViewModel);
        }
    }
}