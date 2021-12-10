namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy;

using Aviant.DDD.Application.UseCases;

public sealed class GetAccountInput : UseCaseInput
{
    public GetAccountInput(Guid id) => Id = id;

    internal Guid Id { get; }
}
