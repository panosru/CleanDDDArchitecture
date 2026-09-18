using CleanDDDArchitecture.Domains.Account.Core.Aggregates;
using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails;

public interface IUpdateDetailsOutput : IUseCaseOutput
{
    public void Ok(AccountAggregate accountAggregate);

    public void Invalid(string message);
}
