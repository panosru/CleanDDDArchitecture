namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Identity;

    public class GetAccountUseCase
        : UseCase<GetAccountInput, IGetAccountOutput>
    {
        public override async Task Execute(GetAccountInput input)
        {
            RequestResult requestResult = await Orchestrator.SendQuery(
                    new GetAccountQuery
                    {
                        Id = input.Id
                    })
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountUser>());
        }
    }
}