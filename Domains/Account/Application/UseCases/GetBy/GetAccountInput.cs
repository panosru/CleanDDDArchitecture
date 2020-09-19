namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System;
    using Aviant.DDD.Application.UseCases;

    public class GetAccountInput : IUseCaseInput
    {
        public GetAccountInput(Guid id) => Id = id;

        public Guid Id { get; }
    }
}