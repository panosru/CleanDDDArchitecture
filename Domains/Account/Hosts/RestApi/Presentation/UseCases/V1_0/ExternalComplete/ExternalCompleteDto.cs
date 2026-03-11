namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.ExternalComplete;

public sealed class ExternalCompleteDto
{
    public string Code { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string RedirectUri { get; set; } = string.Empty;
}
