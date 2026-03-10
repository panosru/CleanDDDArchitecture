using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Sessions;

public interface IListSessionsOutput : IUseCaseOutput
{
    public void Ok(IReadOnlyCollection<AccountSessionDto> sessions);
}
