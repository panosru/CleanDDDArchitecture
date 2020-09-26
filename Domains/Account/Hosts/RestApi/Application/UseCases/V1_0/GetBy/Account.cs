namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using System;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using Domains.Account.Application.Identity;
    using Domains.Account.Application.UseCases.GetBy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account
        : ApiController<GetAccountUseCase, Account>,
          IGetAccountOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Account([FromServices] GetAccountUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IGetAccountOutput Members

        void IGetAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IGetAccountOutput.Ok(AccountUser accountUser) =>
            ViewModel = Ok(new AccountGetByResponse(accountUser));

        #endregion

        // void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
        //     ViewModel = Ok(new AccountGetByResponse(accountAggregate));

        /// <summary>
        ///     Get Account
        /// </summary>
        /// <response code="200">The account.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <returns>Account data.</returns>
        [HttpGet("{id:guid}", Name = "GetAccount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountGetByResponse))]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Find))]
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