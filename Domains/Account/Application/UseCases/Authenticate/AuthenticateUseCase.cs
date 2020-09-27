namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AuthenticateUseCase
        : UseCase<AuthenticateInput, IAuthenticateOutput>
    {
        public override async Task ExecuteAsync(
            AuthenticateInput input,
            CancellationToken cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                    new AuthenticateCommand
                    {
                        Username = input.Username,
                        Password = input.Password
                    },
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded
             && !(requestResult.Payload() is null))
                Output.Ok(requestResult.Payload());
        }
    }
}