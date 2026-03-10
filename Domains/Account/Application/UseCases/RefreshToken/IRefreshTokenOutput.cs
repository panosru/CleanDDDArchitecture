using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.RefreshToken;

public interface IRefreshTokenOutput : IUseCaseOutput
{
    public void Ok(AuthResult response);

    public void Unauthorized();
}
