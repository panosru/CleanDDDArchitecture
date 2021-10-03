namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using Aggregates;
    using Aviant.DDD.Application.UseCases;

    public interface ICreateAccountOutput : IUseCaseOutput
    {
        public void Ok(AccountAggregate accountAggregate);

        public void Invalid(string message);
    }
}
