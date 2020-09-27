namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class UpdateDetailsUseCase
        : UseCase<UpdateDetailsInput, IUpdateDetailsOutput, AccountAggregate, AccountAggregateId>
    {
        public override async Task ExecuteAsync(
            UpdateDetailsInput input,
            CancellationToken  cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendCommandAsync(
                    new UpdateAccount(
                        new AccountAggregateId(input.Id),
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