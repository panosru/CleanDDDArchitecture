namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.DeleteAccount;

public sealed class DeleteAccountConfirmDto
{
    public string Email { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}
