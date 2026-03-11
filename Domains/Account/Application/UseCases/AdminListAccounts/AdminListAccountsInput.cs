using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;

public sealed record AdminListAccountsInput(string? Query, string? Status, string? Role) : UseCaseInput;
