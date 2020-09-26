namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;

    public class UpdateDetailsUseCase
        : UseCase<UpdateDetailsInput, IUpdateDetailsOutput, AccountAggregate, AccountAggregateId>
    {
        public override async Task Execute(UpdateDetailsInput input)
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new UpdateAccount(
                        new AccountAggregateId(input.Id),
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