namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class ConfirmEmailUseCase
        : UseCase<ConfirmEmailInput, IConfirmEmailOutput>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new ConfirmEmailCommand
                    {
                        Token = Input.Token,
                        Email = Input.Email
                    })
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
            {
                var identityResult = requestResult.Payload<IdentityResult>();

                if (identityResult.Succeeded)
                    Output.Ok();
                else
                    Output.Invalid(identityResult.Errors.First());
            }
            else
            {
                Output.Invalid(requestResult.Messages.First());
            }
        }

        public ConfirmEmailUseCase SetInput(ConfirmEmailCommand command)
        {
            Input = new ConfirmEmailInput(command.Token, command.Email);

            return this;
        }
    }
}