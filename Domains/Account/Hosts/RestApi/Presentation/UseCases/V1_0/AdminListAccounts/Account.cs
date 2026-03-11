using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;
using CleanDDDArchitecture.Hosts.RestApi.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.AdminListAccounts;

[ApiVersion("1.0")]
[FeatureGate(Features.AccountAdministration)]
public sealed class Account : ApiController<AdminListAccountsUseCase, Account>, IAdminListAccountsOutput
{
    public Account([FromServices] AdminListAccountsUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IAdminListAccountsOutput.Ok(IReadOnlyCollection<Core.Identity.Dto.AccountAdminSummaryDto> accounts) =>
        ViewModel = Ok(new AccountAdminSummaryResponse(accounts));

    void IAdminListAccountsOutput.Invalid(string message) => ViewModel = BadRequest(message);

    [HttpGet("admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAdminSummaryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> ListAccounts(
        [FromQuery] string? query = null,
        [FromQuery] string? status = null,
        [FromQuery] string? role = null)
    {
        await UseCase.ExecuteAsync(new AdminListAccountsInput(query, status, role)).ConfigureAwait(false);

        return ViewModel;
    }
}
