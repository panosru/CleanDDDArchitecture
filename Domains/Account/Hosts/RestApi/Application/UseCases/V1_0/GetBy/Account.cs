namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.GetBy
{
    using System;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Account.Application.Identity;
    using Domains.Account.Application.UseCases.GetBy;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [ApiVersion("1.0")]
    [FeatureGate(Features.AccountGetBy)]
    [AllowAnonymous]
    public sealed class Account
        : ApiController<GetAccountUseCase, Account>,
          IGetAccountOutput
    {
        /// <inheritdoc />
        public Account([FromServices] GetAccountUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IGetAccountOutput Members

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void IGetAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        /// <summary>
        /// </summary>
        /// <param name="accountUser"></param>
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
        [HttpGet("{id:guid}", Name                          = "GetAccount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountGetByResponse))]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Find))]
        public async Task<IActionResult> GetAccount([FromRoute] Guid id)
        {
            await UseCase.ExecuteAsync(new GetAccountInput(id))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}