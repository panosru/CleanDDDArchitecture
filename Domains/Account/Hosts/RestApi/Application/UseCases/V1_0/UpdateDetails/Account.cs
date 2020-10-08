namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using System;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.UpdateDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <inheritdoc
    ///     cref="CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.ApiController&lt;TUseCase,TUseCaseOutput&gt;" />
    [ApiVersion("1.0")]
    [FeatureGate(Features.AccountUpdateDetails)]
    [AllowAnonymous]
    public sealed class Account
        : ApiController<UpdateDetailsUseCase, Account>,
          IUpdateDetailsOutput
    {
        /// <inheritdoc />
        public Account([FromServices] UpdateDetailsUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IUpdateDetailsOutput Members

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        void IUpdateDetailsOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        /// <summary>
        /// </summary>
        /// <param name="accountAggregate"></param>
        void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
            ViewModel = CreatedAtAction(
                "GetAccount",
                new { id = accountAggregate.Id },
                new AccountUpdateResponse(accountAggregate));

        #endregion

        /// <summary>
        ///     Update account details
        /// </summary>
        /// <response code="200">Returns updated account data.</response>
        /// <response code="204">Account updated</response>
        /// <response code="400">Bad request.</response>
        /// <response code="404">Not Found.</response>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns>Account updated details</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountUpdateResponse))]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Edit))]
        public async Task<IActionResult> Update(
            [FromRoute] Guid             id,
            [FromBody]  UpdateAccountDto dto)
        {
            await UseCase.ExecuteAsync(
                    new UpdateDetailsInput(
                        id,
                        dto.FirstName,
                        dto.LastName,
                        dto.Email))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}