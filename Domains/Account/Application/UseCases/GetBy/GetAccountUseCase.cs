namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Identity;

    public class GetAccountUseCase
        : UseCase<GetAccountInput, IGetAccountOutput>
    {
        public override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                    new GetAccountQuery
                    {
                        Id = Input.Id
                    })
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountUser>());
        }

        public GetAccountUseCase SetInput(Guid id)
        {
            Input = new GetAccountInput(id);

            return this;
        }
    }
}