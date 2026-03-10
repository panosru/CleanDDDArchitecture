using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Deactivate;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.Deactivate;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountDeactivate)]
public sealed class Account : ApiController<DeactivateUseCase, Account>, IDeactivateOutput
{
    public Account([FromServices] DeactivateUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IDeactivateOutput.Ok() => ViewModel = Ok();

    void IDeactivateOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Deactivate([FromBody] DeactivateDto dto)
    {
        await UseCase.ExecuteAsync(new DeactivateInput(dto.CurrentPassword)).ConfigureAwait(false);

        return ViewModel;
    }
}
