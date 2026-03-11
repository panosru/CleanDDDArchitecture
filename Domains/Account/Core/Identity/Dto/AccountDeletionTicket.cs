namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

public sealed record AccountDeletionTicket(
    string Email,
    string FullName,
    string Token);
