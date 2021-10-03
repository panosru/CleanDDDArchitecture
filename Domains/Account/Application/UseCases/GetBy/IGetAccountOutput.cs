namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using Aviant.DDD.Application.UseCases;
    using Identity;

    public interface IGetAccountOutput : IUseCaseOutput
    {
        public void Ok(AccountUser accountUser);

        public void Invalid(string message);
    }
}
