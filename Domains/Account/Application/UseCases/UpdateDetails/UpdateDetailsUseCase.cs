namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Dtos;

    public class UpdateDetailsUseCase
        : UseCase<UpdateDetailsInput, IUpdateDetailsOutput, AccountAggregate, AccountAggregateId>
    {
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                new UpdateAccount(
                    new AccountAggregateId(Input.Id),
                    Input.FirstName,
                    Input.LastName,
                    Input.Email));

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountAggregate>());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            var dto = GetDataByType<UpdateAccountDto>(data);

            Input = new UpdateDetailsInput(
                dto.Id,
                dto.FirstName,
                dto.LastName,
                dto.Email);
        }
    }
}