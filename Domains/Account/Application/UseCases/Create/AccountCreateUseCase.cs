namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class AccountCreateUseCase
        : UseCase<CreateAccountInput, ICreateAccountOutput, AccountAggregate, AccountAggregateId>
    {
        public override async Task ExecuteAsync(
            CreateAccountInput input,
            CancellationToken  cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                    new CreateAccount(
                        input.UserName,
                        input.Password,
                        input.FirstName,
                        input.LastName,
                        input.Email),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountAggregate>());
            else
                Output.Invalid(requestResult.Messages.First());
        }
    }
}