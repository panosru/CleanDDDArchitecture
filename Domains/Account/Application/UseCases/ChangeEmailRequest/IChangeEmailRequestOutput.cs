using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmailRequest;

public interface IChangeEmailRequestOutput : IUseCaseOutput
{
    public void Accepted();
}
