namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AuthenticateUseCase
        : UseCase<AuthenticateInput, IAuthenticateOutput>
    {
        public override async Task Execute(AuthenticateInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new AuthenticateCommand
                    {
                        Username = input.Username,
                        Password = input.Password
                    })
               .ConfigureAwait(false);

            if (requestResult.Succeeded
             && !(requestResult.Payload() is null))
                Output.Ok(requestResult.Payload());
        }
    }
}