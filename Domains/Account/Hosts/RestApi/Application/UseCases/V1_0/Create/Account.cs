namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.Create;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [FeatureGate(Features.AccountCreate)]
    public sealed class Account
        : ApiController<AccountCreateUseCase, Account>,
          ICreateAccountOutput
    {
        /// <summary>
        /// </summary>
        /// <param name="useCase"></param>
        public Account([FromServices] AccountCreateUseCase useCase)
            : base(useCase) => useCase.SetOutput(this);

        #region ICreateAccountOutput Members

        void ICreateAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ICreateAccountOutput.Ok(AccountAggregate accountAggregate) =>
            ViewModel = CreatedAtAction(
                "GetAccount",
                new
                    { id = accountAggregate.Id },
                new AccountCreateResponse(accountAggregate));

        #endregion

        /// <summary>
        ///     Create a new account
        /// </summary>
        /// <response code="200">Account already exists</response>
        /// <response code="201">Account created successfully</response>
        /// <response code="400">Bad request.</response>
        /// <param name="dto"></param>
        /// <returns>The newly registered account.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type      = typeof(AccountCreateResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountCreateResponse))]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Create))]
        public async Task<IActionResult> Create([FromBody] [Required] CreateAccountDto dto)
        {
            await UseCase.ExecuteAsync(
                new CreateAccountInput(
                    dto.UserName,
                    dto.Password,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email))
               .ConfigureAwait(false);

            return ViewModel;
        }
    }
}