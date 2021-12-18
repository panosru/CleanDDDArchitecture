namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;

using Aviant.DDD.Application.UseCases;

public sealed record UpdateDetailsInput(
    Guid   Id,
    string FirstName,
    string LastName,
    string Email) : UseCaseInput
{
    internal Guid Id { get; } = Id;

    internal string FirstName { get; } = FirstName;

    internal string LastName { get; } = LastName;

    internal string Email { get; } = Email;
}
