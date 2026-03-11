using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.DeleteRequest;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountDelete)]
public sealed class Account : ApiController<DeleteAccountRequestUseCase, Account>, IDeleteAccountRequestOutput
{
    public Account([FromServices] DeleteAccountRequestUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IDeleteAccountRequestOutput.Accepted() => ViewModel = Accepted();

    void IDeleteAccountRequestOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpPost("delete/request")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> RequestDeletion([FromBody] DeleteAccountRequestDto dto)
    {
        await UseCase.ExecuteAsync(new DeleteAccountRequestInput(dto.CurrentPassword)).ConfigureAwait(false);

        return ViewModel;
    }
}
