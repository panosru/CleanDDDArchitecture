namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AuthenticateUseCase
        : UseCase<AuthenticateInput, IAuthenticateOutput>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new AuthenticateCommand
                    {
                        Username = Input.Username,
                        Password = Input.Password
                    })
               .ConfigureAwait(false);

            if (requestResult.Succeeded
             && !(requestResult.Payload() is null))
                Output.Ok(requestResult.Payload());
        }

        public AuthenticateUseCase SetInput(AuthenticateCommand command)
        {
            Input = new AuthenticateInput(command.Username, command.Password);

            return this;
        }
    }
}