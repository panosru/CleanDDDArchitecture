namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed class AccountAdminSummaryDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? LastAccessed { get; set; }

    public IReadOnlyCollection<string> Roles { get; set; } = [];
}
