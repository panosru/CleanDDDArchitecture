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
        public override async Task Execute(CreateAccountInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new CreateAccount(
                        input.UserName,
                        input.Password,
                        input.FirstName,
                        input.LastName,
                        input.Email))
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountAggregate>());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}