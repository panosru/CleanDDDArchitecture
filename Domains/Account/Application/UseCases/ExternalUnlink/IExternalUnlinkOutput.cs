using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ExternalUnlink;

public interface IExternalUnlinkOutput : IUseCaseOutput
{
    void Ok();

    void Invalid(string message);
}
