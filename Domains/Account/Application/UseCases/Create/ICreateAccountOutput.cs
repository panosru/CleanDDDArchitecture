using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create;

public interface ICreateAccountOutput : IUseCaseOutput
{
    public void Ok(AccountAggregate accountAggregate);

    public void Invalid(string message);
}
