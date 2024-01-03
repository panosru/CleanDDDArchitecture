using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;

public sealed record GetAccountInput(Guid Id) : UseCaseInput
{
    internal Guid Id { get; } = Id;
}
