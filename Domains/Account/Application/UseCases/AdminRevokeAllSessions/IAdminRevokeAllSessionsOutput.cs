using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.AdminRevokeAllSessions;

public interface IAdminRevokeAllSessionsOutput : IUseCaseOutput
{
    void Ok(int revokedSessions);

    void Invalid(string message);
}
