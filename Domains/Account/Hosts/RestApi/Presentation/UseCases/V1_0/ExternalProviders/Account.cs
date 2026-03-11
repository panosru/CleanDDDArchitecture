using System.Net.Mime;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalProviders;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalProviders;

[ApiVersion("1.0")]
[AllowAnonymous]
[FeatureGate(Features.AccountExternalIdentity)]
public sealed class Account
    : ApiController<ExternalProvidersUseCase, Account>,
      IExternalProvidersOutput
{
    public Account([FromServices] ExternalProvidersUseCase useCase)
        : base(useCase) => UseCase.SetOutput(this);

    void IExternalProvidersOutput.Ok(IReadOnlyCollection<Core.Identity.Dto.ExternalIdentityProviderDto> providers) =>
        ViewModel = Ok(new ExternalProvidersResponse(providers));

    [HttpGet("external/providers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExternalProvidersResponse))]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Providers()
    {
        await UseCase.ExecuteAsync().ConfigureAwait(false);

        return ViewModel;
    }
}
