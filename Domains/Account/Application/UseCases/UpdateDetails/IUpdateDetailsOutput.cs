namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using Aggregates;
    using Aviant.DDD.Application.UseCases;

    public interface IUpdateDetailsOutput : IUseCaseOutput
    {
        public void Ok(AccountAggregate accountAggregate);

        public void Invalid(string message);
    }
}
