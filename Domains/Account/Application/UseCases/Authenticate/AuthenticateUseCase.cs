namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate
{
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AuthenticateUseCase
        : UseCase<AuthenticateInput, IAuthenticateOutput>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new AuthenticateCommand
                {
                    Username = Input.Username,
                    Password = Input.Password
                });

            if (requestResult.Succeeded
             && !(requestResult.Payload() is null))
                Output.Ok(requestResult.Payload());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            var commandData = GetDataByType<AuthenticateCommand>(data);

            Input = new AuthenticateInput(commandData.Username, commandData.Password);
        }
    }
}