namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Dtos;

    public class UpdateDetailsUseCase
        : UseCase<UpdateDetailsInput, IUpdateDetailsOutput, AccountAggregate, AccountAggregateId>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendCommand(
                    new UpdateAccount(
                        new AccountAggregateId(Input.Id),
                        Input.FirstName,
                        Input.LastName,
                        Input.Email))
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountAggregate>());
            else
                Output.Invalid(requestResult.Messages.First());
        }

        public UpdateDetailsUseCase SetInput(Guid id, UpdateAccountDto dto)
        {
            Input = new UpdateDetailsInput(
                id,
                dto.FirstName,
                dto.LastName,
                dto.Email);

            return this;
        }
    }
}