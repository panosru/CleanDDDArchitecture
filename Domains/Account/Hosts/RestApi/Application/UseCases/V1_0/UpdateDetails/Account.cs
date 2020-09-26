namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using System;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.UpdateDetails;
    using Domains.Account.Application.UseCases.UpdateDetails.Dtos;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account
        : ApiController<UpdateDetailsUseCase, Account>,
          IUpdateDetailsOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Account([FromServices] UpdateDetailsUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IUpdateDetailsOutput Members

        void IUpdateDetailsOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

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
            await UseCase
               .SetInput(id, dto)
               .Execute().ConfigureAwait(false);

            return ViewModel;
        }
    }
}