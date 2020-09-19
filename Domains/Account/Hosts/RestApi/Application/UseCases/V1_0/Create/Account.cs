namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application.UseCases.V1_0.Create
{
    using System.Threading.Tasks;
    using CleanDDDArchitecture.Hosts.RestApi.Core.Controllers;
    using Domains.Account.Application.Aggregates;
    using Domains.Account.Application.UseCases.Create;
    using Domains.Account.Application.UseCases.Create.Dtos;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Account endpoints
    /// </summary>
    [ApiVersion("1.0")]
    public sealed class Account : ApiController<AccountCreateUseCase>, ICreateAccountOutput
    {
        public Account([FromServices] AccountCreateUseCase useCase)
            : base(useCase)
        { }

        #region ICreateAccountOutput Members

        void ICreateAccountOutput.Invalid(string message) =>
            ViewModel = BadRequest(message);

        void ICreateAccountOutput.Ok(AccountAggregate accountAggregate) =>
            ViewModel = CreatedAtAction(
                "GetAccount",
                new
                    { id = accountAggregate.Id },
                new AccountResponse(accountAggregate));

        #endregion

        /// <summary>
        ///     Create a new account
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            await UseCase.ExecuteAsync(this, dto);

            return ViewModel;
        }
    }
}