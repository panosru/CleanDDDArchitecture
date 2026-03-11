using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalLogins;

public sealed class ExternalLoginsResponse
{
    public ExternalLoginsResponse(IReadOnlyCollection<ExternalLoginDto> logins) => Logins = logins;

    public IReadOnlyCollection<ExternalLoginDto> Logins { get; }
}
