namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.UpdateDetails
{
    using System;
    using System.Threading.Tasks;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.UpdateDetails;
    using Domains.Account.Application.UseCases.UpdateDetails.Dtos;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account
        : ApiController<UpdateDetailsUseCase, Account>,
          IUpdateDetailsOutput
    {
        public Account([FromServices] UpdateDetailsUseCase useCase)
            : base(useCase) => UseCase.SetOutput(this);

        #region IUpdateDetailsOutput Members

        void IUpdateDetailsOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void IUpdateDetailsOutput.Ok(AccountAggregate accountAggregate) =>
            ViewModel = Ok(new AccountResponse(accountAggregate));

        #endregion

        /// <summary>
        ///     Update account details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid             id,
            [FromBody]  UpdateAccountDto dto)
        {
            await UseCase
               .SetInput(id, dto)
               .Execute()
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}