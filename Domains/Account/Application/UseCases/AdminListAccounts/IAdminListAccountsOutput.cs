using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminListAccounts;

public interface IAdminListAccountsOutput : IUseCaseOutput
{
    void Ok(IReadOnlyCollection<AccountAdminSummaryDto> accounts);

    void Invalid(string message);
}
