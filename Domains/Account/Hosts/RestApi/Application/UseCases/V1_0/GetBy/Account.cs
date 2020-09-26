namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using System;
    using System.Threading.Tasks;
    using Domains.Account.Application.Identity;
    using Domains.Account.Application.UseCases.GetBy;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account
        : ApiController<GetAccountUseCase, Account>,
          IGetAccountOutput
    {
        public Account([FromServices] GetAccountUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IGetAccountOutput Members

        void IGetAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IGetAccountOutput.Ok(AccountUser accountUser) =>
            ViewModel = Ok(new AccountResponse(accountUser));

        #endregion

        // void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
        //     ViewModel = Ok(new AccountResponse(accountAggregate));

        /// <summary>
        ///     Get Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}", Name = "GetAccount")]
        public async Task<IActionResult> GetAccount([FromRoute] Guid id)
        {
            await UseCase
               .SetInput(id)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}