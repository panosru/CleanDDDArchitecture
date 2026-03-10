using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ChangeEmailRequest;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[FeatureGate(Features.AccountChangeEmail)]
public sealed class Account
    : ApiController<ChangeEmailRequestUseCase, Account>,
      IChangeEmailRequestOutput
{
    public Account([FromServices] ChangeEmailRequestUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IChangeEmailRequestOutput.Accepted() =>
        ViewModel = Accepted();

    [HttpPost("change-email/request")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RequestChange([FromBody] ChangeEmailRequestDto dto)
    {
        await UseCase.ExecuteAsync(new ChangeEmailRequestInput(dto.NewEmail))
            .ConfigureAwait(false);

        return ViewModel;
    }
}
