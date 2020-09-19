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
        protected override async Task Execute()
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                new GetAccountQuery
                {
                    Id = Input.Id
                });

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountUser>());
        }

        protected override void SetInput<TInputData>(TInputData data)
        {
            Input = new GetAccountInput(GetDataByType<Guid>(data));
        }
    }
}