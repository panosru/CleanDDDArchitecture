namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Identity;

    public class GetAccountUseCase
        : UseCase<GetAccountInput, IGetAccountOutput>
    {
        public override async Task ExecuteAsync(
            GetAccountInput   input,
            CancellationToken cancellationToken = default)
        {
            RequestResult requestResult = await Orchestrator.SendQueryAsync(
                    new GetAccountQuery
                    {
                        Id = input.Id
                    },
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountUser>());
        }
    }
}