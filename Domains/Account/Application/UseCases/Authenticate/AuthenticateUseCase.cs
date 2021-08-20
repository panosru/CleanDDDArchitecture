namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public sealed class AuthenticateUseCase
        : UseCase<AuthenticateInput, IAuthenticateOutput>
    {
        public override async Task ExecuteAsync(
            AuthenticateInput input,
            CancellationToken cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendCommandAsync(
                    new AuthenticateCommand(
                        input.Username,
                        input.Password),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded
             && requestResult.Payload() is not null)
                Output.Ok(requestResult.Payload());
            else
                Output.Unauthorized();
        }
    }
}