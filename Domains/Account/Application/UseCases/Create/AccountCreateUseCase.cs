namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AccountCreateUseCase
        : UseCase<CreateAccountInput, ICreateAccountOutput, AccountAggregate, AccountAggregateId>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new CreateAccount(
                        Input.UserName,
                        Input.Password,
                        Input.FirstName,
                        Input.LastName,
                        Input.Email))
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountAggregate>());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        public AccountCreateUseCase SetInput(CreateAccountDto dto)
        {
            Input = new CreateAccountInput(
                dto.UserName,
                dto.Password,
                dto.FirstName,
                dto.LastName,
                dto.Email);

            return this;
        }
    }
}