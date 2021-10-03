namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.GetBy
{
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Identity;

    public sealed class GetAccountUseCase
        : UseCase<GetAccountInput, IGetAccountOutput>
    {
        public override async Task ExecuteAsync(
            GetAccountInput   input,
            CancellationToken cancellationToken = default)
        {
            OrchestratorResponse requestResult = await Orchestrator.SendQueryAsync(
                    new GetAccountQuery(input.Id),
                    cancellationToken)
               .ConfigureAwait(false);

            if (requestResult.Succeeded)
                Output.Ok(requestResult.Payload<AccountUser>());
        }
    }
}
