namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;

using Aviant.DDD.Application.UseCases;

public sealed record GetAccountInput(Guid Id) : UseCaseInput
{
    internal Guid Id { get; } = Id;
}
