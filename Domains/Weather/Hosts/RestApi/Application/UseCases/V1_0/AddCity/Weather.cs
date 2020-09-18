namespace CleanDDDArchitecture.Domains.Weather.Hosts.RestApi.Application.UseCases.V1_0.AddCity
{
    using System.Threading.Tasks;
    using Domains.Weather.Application.UseCases.AddCity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Weather endpoints
    /// </summary>
    public class Weather : ApiController<AddCityUseCase>, IAddCityOutput
    {
        public Weather([FromServices] AddCityUseCase useCase)
            : base(useCase)
        { }

        #region IAddCityOutput Members

        void IAddCityOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IAddCityOutput.Ok(string city) =>
            ViewModel = Ok(new AddCityResponse(city).ToString());

        void IAddCityOutput.NotFound() =>
            ViewModel = NotFound();

        #endregion

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCity([FromBody] AddCityDto dto)
        {
            await UseCase.ExecuteAsync(this, dto);

            return ViewModel;
        }
    }
}